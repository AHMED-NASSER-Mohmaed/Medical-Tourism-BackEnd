using AutoMapper;
using Elagy.Core.DTOs.CarRentals;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.TOP;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.Helpers;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class CarService : ICarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CarService> _logger;
        private readonly IFileStorageService _fileStorageService;

        public CarService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<CarService> logger,
            IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileStorageService = fileStorageService;
        }

        // --- Car Rental Admin Dashboard Car Management ---
        public async Task<PagedResponseDto<CarResponseDto>> GetAllCarsForCarRentalAdminAsync(string carRentalAssetId, PaginationParameters paginationParameters)
        {
            try
            {
                var cars = await _unitOfWork.Cars.GetCarsByCarRentalAssetIdAsync(carRentalAssetId);

                // 2. Apply filters from PaginationParameters
                IQueryable<Car> query = cars.AsQueryable();

                // Search by make, model, description
                if (!string.IsNullOrWhiteSpace(paginationParameters.SearchTerm))
                {
                    string term = paginationParameters.SearchTerm.Trim().ToLower();
                    query = query.Where(c =>
                        c.FactoryMake.ToLower().Contains(term) ||
                        c.ModelName.ToLower().Contains(term) ||
                        c.Description.ToLower().Contains(term)
                    );
                }

                // Filter by CarType
                if (paginationParameters.CarType.HasValue)
                {
                    query = query.Where(c => c.Type == paginationParameters.CarType.Value);
                }

                // Filter by CarStatus
                if (paginationParameters.CarStatus.HasValue)
                {
                    query = query.Where(c => c.Status == paginationParameters.CarStatus.Value);
                }

                // Filter by IsAvailable (general operational availability)
                if (paginationParameters.FilterIsActive.HasValue)
                {
                    query = query.Where(c => c.IsAvailable == paginationParameters.FilterIsActive.Value);
                }

                // Filter by Price range
                if (paginationParameters.MinPrice.HasValue)
                {
                    query = query.Where(c => c.PricePerDay >= paginationParameters.MinPrice.Value);
                }
                if (paginationParameters.MaxPrice.HasValue)
                {
                    query = query.Where(c => c.PricePerDay <= paginationParameters.MaxPrice.Value);
                }
                
                if (paginationParameters.MinOccupancy.HasValue)
                {
                    query = query.Where(c => c.Capacity >= paginationParameters.MinOccupancy.Value);
                }
                if (paginationParameters.MaxOccupancy.HasValue)
                {
                    query = query.Where(c => c.Capacity <= paginationParameters.MaxOccupancy.Value);
                }

                if (paginationParameters.FilterGovernorateId.HasValue)
                {
                    query = query.Where(c => c.CarRentalAsset.ServiceProvider.GovernorateId == paginationParameters.FilterGovernorateId.Value);
                }

                var totalCount = query.Count();

                var pagedCars =  query
                    .OrderBy(c => c.ModelName) // Default sorting by model name
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToList();

                var carDtos = _mapper.Map<IEnumerable<CarResponseDto>>(pagedCars);
                return new PagedResponseDto<CarResponseDto>(carDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error getting all cars for Car Rental Admin Dashboard for Asset ID: {carRentalAssetId}."); throw; }
        }

        public async Task<CarResponseDto> CreateCarAsync(CarCreateDto createDto, string carRentalAssetId, List<IFormFile>? imageFiles = null)
        {
            try
            {
                // Validate CarRentalAsset existence.
                var carRentalAsset = await _unitOfWork.CarRentalAssets.GetByIdAsync(carRentalAssetId);
                if (carRentalAsset == null) throw new ArgumentException($"Car Rental Asset with ID {carRentalAssetId} not found.");

                //Map DTO to Car entity.
                var car = _mapper.Map<Car>(createDto);
                car.CarRentalAssetId = carRentalAssetId;
                car.IsAvailable = true; 
                car.Status = CarStatus.Available; 

                //Handle Image Uploads (using IFileStorageService)
                if (imageFiles != null && imageFiles.Any())
                {
                    var uploadResult = await _fileStorageService.UploadMultipleFilesAsync(imageFiles);
                    if (uploadResult.OverallSuccess && uploadResult.UploadResults.Any(r => r.Success))
                    {
                        car.CarImages = uploadResult.UploadResults.Where(r => r.Success)
                            .Select((uploadedFile,index) => new CarImage { ImageId = uploadedFile.Id, ImageURL = uploadedFile.Url, CarId = car.Id}).ToList();
                        _logger.LogInformation($"Successfully uploaded {car.CarImages.Count} images for new car.");
                    }
                    else { _logger.LogError($"Some or all images failed to upload for new car. Details: {uploadResult.Message}"); }
                }
                else { car.CarImages = new List<CarImage>(); } 

                //Add car to repository and persist.
                await _unitOfWork.Cars.AddAsync(car);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Car '{car.FactoryMake} {car.ModelName}' (ID: {car.Id}) created successfully for Rental Asset {carRentalAssetId}.");

                var createdCarWithDetails = await _unitOfWork.Cars.GetCarByIdWithDetailsAsync(car.Id);
                if (createdCarWithDetails == null) throw new Exception("Created car not found after save.");

                return _mapper.Map<CarResponseDto>(createdCarWithDetails);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error creating car for Car Rental Asset {carRentalAssetId}."); throw; }
        }

        public async Task<CarResponseDto> UpdateCarAsync(int carId, CarUpdateDto updateDto, string carRentalAssetId, List<IFormFile>? newImageFiles = null)
        {
            try
            {

                var car = await _unitOfWork.Cars.GetCarByIdWithDetailsAsync(carId);
                if (car == null) throw new KeyNotFoundException($"Car with ID {carId} not found for update.");

                if (car.CarRentalAssetId != carRentalAssetId) throw new UnauthorizedAccessException($"Car with ID {carId} is not affiliated with your Car Rental Asset.");

                if (updateDto.Capacity.HasValue && updateDto.Capacity.Value <= 0) throw new ArgumentException("Capacity must be positive.");

                if (newImageFiles != null && newImageFiles.Any())
                {
                    // Delete existing images from cloud storage.
                    if (car.CarImages != null && car.CarImages.Any())
                    {
                        var oldImageCloudIds = car.CarImages.Select(ci => ci.ImageId).ToList();
                        var deleteResult = await _fileStorageService.DeleteMultipleFilesAsync(oldImageCloudIds);
                        if (!deleteResult.OverallSuccess) _logger.LogWarning($"Failed to delete some old images for car {carId} from Imagekit. Details: {deleteResult.Message}");
                    }
                    // Clear existing images from the tracked entity's collection.
                    if (car.CarImages != null) car.CarImages.Clear();

                    // Upload new image files.
                    var uploadResult = await _fileStorageService.UploadMultipleFilesAsync(newImageFiles, $"cars/{carRentalAssetId}/images");
                    if (uploadResult.OverallSuccess && uploadResult.UploadResults.Any(r => r.Success))
                    {
                        car.CarImages = uploadResult.UploadResults.Where(r => r.Success)
                            .Select(uploadedFile => new CarImage { ImageId = uploadedFile.Id, ImageURL = uploadedFile.Url, CarId = car.Id }).ToList();
                        _logger.LogInformation($"Successfully uploaded {car.CarImages.Count} new images for car {car.FactoryMake} {car.ModelName}.");
                    }
                    else { _logger.LogError($"Some or all new images failed to upload for car {carId}. Details: {uploadResult.Message}"); }
                }


                _mapper.Map(updateDto, car);

                // 6. Update the entity in the repository and save changes.
                _unitOfWork.Cars.Update(car);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Car '{car.FactoryMake} {car.ModelName}' (ID: {car.Id}) updated successfully for Rental Asset {carRentalAssetId}.");

                // 7. Re-fetch for comprehensive response DTO.
                var updatedCarWithDetails = await _unitOfWork.Cars.GetCarByIdWithDetailsAsync(carId);
                if (updatedCarWithDetails == null) throw new Exception("Updated car not found after save.");

                return _mapper.Map<CarResponseDto>(updatedCarWithDetails);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error updating car ID: {carId} for Car Rental Asset {carRentalAssetId}."); throw; }
        }


        public async Task<CarResponseDto> ChangeCarAvailabilityStatusAsync(int carId, bool newIsAvailableStatus, string carRentalAssetId)
        {
            try
            {
                var car = await _unitOfWork.Cars.GetCarByIdWithDetailsAsync(carId);
                if (car == null) throw new KeyNotFoundException($"Car with ID {carId} not found for availability status change.");
                if (car.CarRentalAssetId != carRentalAssetId) throw new UnauthorizedAccessException($"Car with ID {carId} is not affiliated with your Car Rental Asset.");
                if (car.IsAvailable == newIsAvailableStatus) { _logger.LogInformation($"Car with ID {carId} is already in the target IsAvailable status ({newIsAvailableStatus}). No action needed."); return _mapper.Map<CarResponseDto>(car); }

                // Validation: Cannot set IsAvailable to false if the car is currently OnRide.
                if (newIsAvailableStatus == false && car.Status == CarStatus.OnRide)
                {
                    throw new InvalidOperationException($"Cannot set car {carId} to unavailable while it is currently 'On Ride'.");
                }

                car.IsAvailable = newIsAvailableStatus;
                _unitOfWork.Cars.Update(car);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Car '{carId}' IsAvailable status changed to {newIsAvailableStatus} successfully for Car Rental Asset {carRentalAssetId}.");
                var updatedCarWithDetails = await _unitOfWork.Cars.GetCarByIdWithDetailsAsync(carId);
                if (updatedCarWithDetails == null) throw new Exception("Updated car not found after save.");
                return _mapper.Map<CarResponseDto>(updatedCarWithDetails);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error changing availability status for car ID: {carId} for Car Rental Asset {carRentalAssetId}."); throw; }
        }
        public async Task<CarResponseDto> ChangeCarInternalStatusAsync(int carId, CarStatus newInternalStatus, string carRentalAssetId)
        {
            try
            {
                var car = await _unitOfWork.Cars.GetCarByIdWithDetailsAsync(carId);
                if (car == null) throw new KeyNotFoundException($"Car with ID {carId} not found for internal status change.");
                if (car.CarRentalAssetId != carRentalAssetId) throw new UnauthorizedAccessException($"Car with ID {carId} is not affiliated with your Car Rental Asset.");
                if (car.Status == newInternalStatus) { _logger.LogInformation($"Car with ID {carId} is already in the target internal status ({newInternalStatus}). No action needed."); return _mapper.Map<CarResponseDto>(car); }

                // Specific validation for status transitions
                if (newInternalStatus == CarStatus.OnRide)
                {
                    if (car.Status != CarStatus.Available) throw new InvalidOperationException($"Car {carId} cannot go 'On Ride' from current status '{car.Status}'. It must be 'Available'.");
                    if (car.IsAvailable == false) throw new InvalidOperationException($"Car {carId} cannot go 'On Ride' because it is marked as generally unavailable.");
             
                     var activeDriverAssignment = await _unitOfWork.CarDrivers.GetAssignmentsByCarIdAsync(carId);
                     if (activeDriverAssignment == null) throw new InvalidOperationException($"Car {carId} cannot go 'On Ride' without an assigned driver.");
                }
                else if (newInternalStatus == CarStatus.Available)
                {
                    if (car.Status == CarStatus.OnRide) throw new InvalidOperationException($"Car {carId} cannot be made 'Available' while it is 'On Ride'. It must return from ride first.");
                    if (car.Status == CarStatus.UnderMaintenance) throw new InvalidOperationException($"Car {carId} cannot be made 'Available' while 'Under Maintenance'. It must be fixed first.");
                }
                else if (newInternalStatus == CarStatus.UnderMaintenance)
                {
                    if (car.Status == CarStatus.OnRide) throw new InvalidOperationException($"Car {carId} cannot go 'Under Maintenance' while it is 'On Ride'. It must return from ride first.");
                    car.IsAvailable = false;
                }

                car.Status = newInternalStatus;
                _unitOfWork.Cars.Update(car);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Car '{carId}' internal status changed to {newInternalStatus} successfully for Car Rental Asset {carRentalAssetId}.");
                var updatedCarWithDetails = await _unitOfWork.Cars.GetCarByIdWithDetailsAsync(carId);
                if (updatedCarWithDetails == null) throw new Exception("Updated car not found after save.");
                return _mapper.Map<CarResponseDto>(updatedCarWithDetails);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error changing internal status for car ID: {carId} for Car Rental Asset {carRentalAssetId}."); throw; }
        }

        // --- Public/Website Car Display & Details ---

        public async Task<PagedResponseDto<CarResponseDto>> GetAvailableCarsForWebsiteAsync(PaginationParameters paginationParameters, string CarRentalAssetId)
        {
            try
            {
                IQueryable<Car> query = _unitOfWork.Cars.AsQueryable();

                query = query.Where(c => c.IsAvailable == true && c.Status == CarStatus.Available);

                // Apply optional filters from PaginationParameters
                if (!string.IsNullOrWhiteSpace(CarRentalAssetId))
                {
                    query = query.Where(c => c.CarRentalAssetId == CarRentalAssetId);
                }

                // Filter by CarType
                if (paginationParameters.CarType.HasValue)
                {
                    query = query.Where(c => c.Type == paginationParameters.CarType.Value);
                }

                // Filter by MaxCapacity (MaxOccupancy in PaginationParameters)
                if (paginationParameters.MinOccupancy.HasValue)
                {
                    query = query.Where(c => c.Capacity >= paginationParameters.MinOccupancy.Value);
                }
                if (paginationParameters.MaxOccupancy.HasValue)
                {
                    query = query.Where(c => c.Capacity <= paginationParameters.MaxOccupancy.Value);
                }

                // Filter by Price range
                if (paginationParameters.MinPrice.HasValue)
                {
                    query = query.Where(c => c.PricePerDay >= paginationParameters.MinPrice.Value);
                }
                if (paginationParameters.MaxPrice.HasValue)
                {
                    query = query.Where(c => c.PricePerDay <= paginationParameters.MaxPrice.Value);
                }

                // SearchTerm on Car properties (e.g., Make, ModelName, Description)
                if (!string.IsNullOrWhiteSpace(paginationParameters.SearchTerm))
                {
                    string term = paginationParameters.SearchTerm.Trim().ToLower();
                    query = query.Where(c =>
                        c.FactoryMake.ToLower().Contains(term) ||
                        c.ModelName.ToLower().Contains(term) ||
                        c.Description.ToLower().Contains(term)
                    );
                }

                // Filter by Governorate (requires linking to CarRentalAsset's ServiceProvider's Governorate)
                if (paginationParameters.FilterGovernorateId.HasValue)
                {
                    query = query.Where(c => c.CarRentalAsset.ServiceProvider.GovernorateId == paginationParameters.FilterGovernorateId.Value);
                }

                var totalCount = await query.CountAsync();

                var pagedCars = await query
                    .Include(c => c.CarRentalAsset)
                        .ThenInclude(cra => cra.ServiceProvider)
                            .ThenInclude(sp => sp.Governorate) 
                                .ThenInclude(g => g.Country)
                    .Include(c => c.CarImages) 
                    .OrderBy(c => c.PricePerDay) 
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                var carDtos = _mapper.Map<IEnumerable<CarResponseDto>>(pagedCars);
                return new PagedResponseDto<CarResponseDto>(carDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
            catch (Exception ex) { _logger.LogError(ex, "Error getting available cars for website."); throw; }
        }

        public async Task<CarResponseDto?> GetCarByIdAsync(int carId)
        {
            try
            {
                var car = await _unitOfWork.Cars.GetCarByIdWithDetailsAsync(carId);
                if (car == null) { _logger.LogInformation($"Car with ID {carId} not found."); return null; }
                return _mapper.Map<CarResponseDto>(car);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error getting car by ID: {carId}."); throw; }
        }

        public async Task<IEnumerable<CarBookingCountDto>> GetTopCarsByRentals()
        {
            return await _unitOfWork.Cars.AsQueryable()
                .Where(s => s.IsAvailable && s.Status != CarStatus.UnderMaintenance)
                .Select(car => new CarBookingCountDto
                {
                    CarId = car.Id,
                    CarRentalId = car.CarRentalAssetId,
                    CarRentalName = car.CarRentalAsset.ServiceProvider.ServiceAsset.Name,
                    ImageUrl = car.CarImages.FirstOrDefault().ImageURL,
                    Make = car.FactoryMake,
                    Model = car.ModelName,
                    Year = car.ModelYear,
                    RentalCount = car.CarRentalAsset.Cars.SelectMany(s => s.carRentalSchedules)
                    .Count(s => s.Status != ScheduleStatus.Cancelled)

                }).ToListAsync();
        }
    }
}
