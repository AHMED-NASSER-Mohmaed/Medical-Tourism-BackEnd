using AutoMapper;
using Elagy.Core.DTOs.Driver;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class CarDriverAssignmentService : ICarDriverAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CarDriverAssignmentService> _logger;

        public CarDriverAssignmentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CarDriverAssignmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<CarDriverResponseDto> AssignDriverToCarAsync(CarDriverCreateDto createDto, string carRentalAssetId)
        {
            try
            {
                var car = await _unitOfWork.Cars.GetCarByIdWithDetailsAsync(createDto.CarId);
                if (car == null) throw new ArgumentException($"Car with ID {createDto.CarId} not found.");

                var driver = await _unitOfWork.Drivers.GetDriverByIdWithDetailsAsync(createDto.DriverId);
                if (driver == null) throw new ArgumentException($"Driver with ID {createDto.DriverId} not found.");

                if (car.CarRentalAssetId != carRentalAssetId) throw new UnauthorizedAccessException($"Car {createDto.CarId} is not affiliated with your Car Rental Asset.");
                if (driver.CarRentalAssetId != carRentalAssetId) throw new UnauthorizedAccessException($"Driver {createDto.DriverId} is not affiliated with your Car Rental Asset.");

                var currentDriverAssignment = await _unitOfWork.Drivers.GetCurrentCarAssignmentForDriverAsync(createDto.DriverId);
                if (currentDriverAssignment != null)
                {
                    throw new InvalidOperationException($"Driver {createDto.DriverId} is currently assigned to car {currentDriverAssignment.Car.FactoryMake} {currentDriverAssignment.Car.ModelName} (ID: {currentDriverAssignment.CarId}). Release them first.");
                }

                if (car.Status != CarStatus.Available || car.IsAvailable == false)
                {
                    throw new InvalidOperationException($"Car {createDto.CarId} is not 'Available' for assignment. Current status: {car.Status}.");
                }

                var existingAssignment = await _unitOfWork.CarDrivers.FindAsync(
                    cd => cd.CarId == createDto.CarId && cd.DriverId == createDto.DriverId && cd.IsAssignedCurrent == false
                ).ContinueWith(t => t.Result.FirstOrDefault()); // Materialize for checking

                CarDriver assignmentToPersist;

                if (existingAssignment != null)
                {
                    _logger.LogInformation($"Reactivating existing assignment ID {existingAssignment.Id} for Car {createDto.CarId} and Driver {createDto.DriverId}.");
                    assignmentToPersist = existingAssignment;
                    assignmentToPersist.IsAssignedCurrent = true;
                    assignmentToPersist.AssignmentDate = DateOnly.FromDateTime(DateTime.Now); 
                    assignmentToPersist.ReleaseDate = null;
                    _unitOfWork.CarDrivers.Update(assignmentToPersist); 
                }
                else
                {
                    // If no existing record, create a new one.
                    _logger.LogInformation($"Creating new assignment for Car {createDto.CarId} and Driver {createDto.DriverId}.");
                    assignmentToPersist = _mapper.Map<CarDriver>(createDto);
                    assignmentToPersist.CarRentalAssetId = carRentalAssetId;
                    assignmentToPersist.AssignmentDate = DateOnly.FromDateTime(DateTime.Now);
                    assignmentToPersist.IsAssignedCurrent = true;
                    assignmentToPersist.ReleaseDate = null; 
                    await _unitOfWork.CarDrivers.AddAsync(assignmentToPersist); 
                }

                await _unitOfWork.CompleteAsync(); 

               
                car.Status = CarStatus.OnRide;
                driver.DriverStatus = DriverStatus.OnDrive;
                _unitOfWork.Cars.Update(car);
                _unitOfWork.Drivers.Update(driver);
                await _unitOfWork.CompleteAsync(); 

                _logger.LogInformation($"Driver {createDto.DriverId} assigned to Car {createDto.CarId} for Car Rental Asset {carRentalAssetId}. Assignment ID: {assignmentToPersist.Id}");

                var finalAssignmentWithDetails = await _unitOfWork.CarDrivers.GetCarDriverByIdWithDetailsAsync(assignmentToPersist.Id);
                if (finalAssignmentWithDetails == null) throw new Exception("Final assignment not found after save.");

                return _mapper.Map<CarDriverResponseDto>(finalAssignmentWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning Driver {createDto.DriverId} to Car {createDto.CarId} for Car Rental Asset {carRentalAssetId}.");
                throw;
            }
        }
        public async Task<CarDriverResponseDto?> ReleaseDriverFromCarAsync(int carId, string driverId, string carRentalAssetId)
        {
            try
            {

                var assignment = await _unitOfWork.CarDrivers.GetAssignmentsByCarIdAsync(carId, isCurrent: true);

                var specificAssignment = assignment.FirstOrDefault(a => a.DriverId == driverId);

                if (specificAssignment == null) throw new KeyNotFoundException($"Active assignment not found for Car {carId} and Driver {driverId}.");



                if (specificAssignment.CarRentalAssetId != carRentalAssetId) throw new UnauthorizedAccessException($"Assignment {specificAssignment.Id} is not affiliated with your Car Rental Asset.");


                specificAssignment.IsAssignedCurrent = false;
                specificAssignment.ReleaseDate = DateOnly.FromDateTime(DateTime.Now);

                _unitOfWork.CarDrivers.Update(specificAssignment);


                var car = await _unitOfWork.Cars.GetByIdAsync(carId);
                var driver = await _unitOfWork.Drivers.GetByIdAsync(driverId);
                if (car == null || driver == null) { throw new Exception("Car or Driver not found during assignment release cleanup."); } // Should not happen


                if (car.Status == CarStatus.OnRide) car.Status = CarStatus.Available;
                if (driver.DriverStatus == DriverStatus.OnDrive) driver.DriverStatus = DriverStatus.Available;

                _unitOfWork.Cars.Update(car);
                _unitOfWork.Drivers.Update(driver);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Driver {driverId} released from Car {carId}. Assignment ID: {specificAssignment.Id}");


                var updatedAssignmentWithDetails = await _unitOfWork.CarDrivers.GetCarDriverByIdWithDetailsAsync(specificAssignment.Id);
                return _mapper.Map<CarDriverResponseDto>(updatedAssignmentWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error releasing Driver {driverId} from Car {carId} for Car Rental Asset {carRentalAssetId}.");
                throw;
            }
        }
        public async Task<CarDriverResponseDto?> GetCurrentAssignmentForDriverAsync(string driverId, string carRentalAssetId)
        {
            try
            {
                var assignment = await _unitOfWork.Drivers.GetCurrentCarAssignmentForDriverAsync(driverId);
                if (assignment == null) return null;

                if (assignment.CarRentalAssetId != carRentalAssetId) throw new UnauthorizedAccessException($"Assignment for Driver {driverId} is not affiliated with your Car Rental Asset.");

                return _mapper.Map<CarDriverResponseDto>(assignment);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error getting current assignment for Driver {driverId}."); throw; }
        }

        public async Task<PagedResponseDto<CarDriverResponseDto>> GetAssignmentsForCarRentalAssetAsync(string carRentalAssetId, PaginationParameters paginationParameters)
        {
            try
            {
                IQueryable<CarDriver> query = _unitOfWork.CarDrivers.AsQueryable();

               
                // Filter by CarRentalAssetId (mandatory for this method)

                if (!string.IsNullOrWhiteSpace(paginationParameters.FilterDoctorId)) // Assuming FilterDriverId for driver ID
                {
                    query = query.Where(cd => cd.DriverId == paginationParameters.FilterDoctorId); // Using FilterDoctorId for DriverId for now
                }
                if (paginationParameters.FilterIsActive.HasValue) //assume for current assignment
                {
                    query = query.Where(cd => cd.IsAssignedCurrent == paginationParameters.FilterIsActive.Value);
                }

                var totalCount = await query.CountAsync();

                var pagedAssignments = await query
                    .Include(cd => cd.Car) // Needed for Car details in DTO
                    .Include(cd => cd.Driver) // Needed for Driver details in DTO
                    .Include(cd => cd.CarRentalAsset) // Needed for CarRentalAssetName
                    .OrderByDescending(cd => cd.AssignmentDate) // Default sorting
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                var assignmentDtos = _mapper.Map<IEnumerable<CarDriverResponseDto>>(pagedAssignments);
                return new PagedResponseDto<CarDriverResponseDto>(assignmentDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error getting assignments for Car Rental Asset {carRentalAssetId}."); throw; }
        }

        /// <summary>
        /// Retrieves assignment history for a specific car.
        /// </summary>
        /// <param name="carId">The ID of the car.</param>
        /// <param name="carRentalAssetId">The ID of the car rental asset.</param>
        /// <param name="paginationParameters">Pagination and filtering parameters.</param>
        /// <returns>A paginated list of CarDriverResponseDto.</returns>
        public async Task<PagedResponseDto<CarDriverResponseDto>> GetAssignmentHistoryForCarAsync(int carId, string carRentalAssetId, PaginationParameters paginationParameters)
        {
            try
            {
                // Start with assignments for this Car and CarRentalAsset
                IQueryable<CarDriver> query = _unitOfWork.CarDrivers.AsQueryable()
                    .Where(cd => cd.CarId == carId && cd.CarRentalAssetId == carRentalAssetId);

                if (paginationParameters.FilterIsActive.HasValue)
                {
                    query = query.Where(cd => cd.IsAssignedCurrent == paginationParameters.FilterIsActive.Value);
                }

                var totalCount = await query.CountAsync();

                var pagedAssignments = await query
                    .Include(cd => cd.Car)
                    .Include(cd => cd.Driver)
                    .Include(cd => cd.CarRentalAsset)
                    .OrderByDescending(cd => cd.AssignmentDate)
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                var assignmentDtos = _mapper.Map<IEnumerable<CarDriverResponseDto>>(pagedAssignments);
                return new PagedResponseDto<CarDriverResponseDto>(assignmentDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error getting assignment history for Car {carId}."); throw; }
        }

        /// <summary>
        /// Retrieves assignment history for a specific driver.
        /// </summary>
        /// <param name="driverId">The ID of the driver.</param>
        /// <param name="carRentalAssetId">The ID of the car rental asset.</param>
        /// <param name="paginationParameters">Pagination and filtering parameters.</param>
        /// <returns>A paginated list of CarDriverResponseDto.</returns>
        public async Task<PagedResponseDto<CarDriverResponseDto>> GetAssignmentHistoryForDriverAsync(string driverId, string carRentalAssetId, PaginationParameters paginationParameters)
        {
            try
            {
                // Start with assignments for this Driver and CarRentalAsset
                IQueryable<CarDriver> query = _unitOfWork.CarDrivers.AsQueryable()
                    .Where(cd => cd.DriverId == driverId && cd.CarRentalAssetId == carRentalAssetId);

                // Apply optional filters from PaginationParameters (e.g., by CarId, IsAssignedCurrent)

                if (paginationParameters.FilterIsActive.HasValue)
                {
                    query = query.Where(cd => cd.IsAssignedCurrent == paginationParameters.FilterIsActive.Value);
                }

                var totalCount = await query.CountAsync();

                var pagedAssignments = await query
                    .Include(cd => cd.Car)
                    .Include(cd => cd.Driver)
                    .Include(cd => cd.CarRentalAsset)
                    .OrderByDescending(cd => cd.AssignmentDate)
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                var assignmentDtos = _mapper.Map<IEnumerable<CarDriverResponseDto>>(pagedAssignments);
                return new PagedResponseDto<CarDriverResponseDto>(assignmentDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error getting assignment history for Driver {driverId}."); throw; }
        }
    }

}
