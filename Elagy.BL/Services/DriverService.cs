using AutoMapper;
using Elagy.Core.DTOs.Driver;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.Helpers;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class DriverService : IDriverService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;
        private readonly ILogger<DriverService> _logger;
        private readonly UserManager<User> _userManager;
        public DriverService(IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<DriverService> logger,
            UserManager<User> userManager,
            IFileStorageService fileStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _fileStorageService = fileStorageService;
        }
        public async Task<DriverResponseDto> ChangeDriverInternalStatusAsync(string driverId, DriverStatus newDriverStatus, string carRentalAssetId)
        {
            try
            {
                var driver = await _unitOfWork.Drivers.GetDriverByIdWithDetailsAsync(driverId);
                if (driver == null) throw new KeyNotFoundException($"Driver with ID {driverId} not found for internal status change.");
                if (driver.CarRentalAssetId != carRentalAssetId) throw new UnauthorizedAccessException($"Driver with ID {driverId} is not affiliated with your Car Rental Asset.");
                if (driver.DriverStatus == newDriverStatus) { _logger.LogInformation($"Driver with ID {driverId} is already in the target internal status ({newDriverStatus}). No action needed."); return _mapper.Map<DriverResponseDto>(driver); }

                
                if (newDriverStatus == DriverStatus.OnDrive)
                {
                    // If moving to OnDrive, driver must have a current assignment and car must be available
                    var currentAssignment = await _unitOfWork.Drivers.GetCurrentCarAssignmentForDriverAsync(driverId);
                    if (currentAssignment == null) throw new InvalidOperationException($"Driver {driverId} cannot be set to 'On Drive' without an active car assignment.");
                    if (currentAssignment.Car.Status != CarStatus.Available) throw new InvalidOperationException($"Cannot set driver {driverId} to 'On Drive' as assigned car {currentAssignment.Car.Id} is not available (Status: {currentAssignment.Car.Status}).");
                }
                else if (newDriverStatus == DriverStatus.Available)
                {
                    // If moving to Available, driver should not have an active assignment.
                    var currentAssignment = await _unitOfWork.Drivers.GetCurrentCarAssignmentForDriverAsync(driverId);
                    if (currentAssignment != null) throw new InvalidOperationException($"Driver {driverId} cannot be set to 'Available' while still assigned to car {currentAssignment.Car.Id}. Release them first.");
                }

                driver.DriverStatus = newDriverStatus;
                var userUpdateResult = await _userManager.UpdateAsync(driver); // Update User base fields, including Status
                if (!userUpdateResult.Succeeded) throw new InvalidOperationException($"Failed to change driver status: {string.Join(", ", userUpdateResult.Errors.Select(e => e.Description))}");

                await _unitOfWork.CompleteAsync(); // Save specific Driver entity changes

                _logger.LogInformation($"Driver '{driverId}' internal status changed to {newDriverStatus} successfully for Car Rental Asset {carRentalAssetId}.");
                return _mapper.Map<DriverResponseDto>(driver);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error changing internal status for driver ID: {driverId} for Car Rental Asset {carRentalAssetId}."); throw; }

        }

    
        public async Task<DriverResponseDto> CreateDriverAsync(DriverCreateDto createDto, string carRentalAssetId, IFormFile? licenseDocument, IFormFile? profileImage)
        {
            try
            {
              
                var carRentalAsset = await _unitOfWork.CarRentalAssets.GetByIdAsync(carRentalAssetId);
                if (carRentalAsset == null) throw new ArgumentException($"Car Rental Asset with ID {carRentalAssetId} not found.");
                if (carRentalAsset.Id != carRentalAssetId) throw new UnauthorizedAccessException($"You can only create drivers for your own Car Rental Asset ({carRentalAssetId}).");


                var userGovernorate = await _unitOfWork.Governates.GetByIdAsync(createDto.UserGovernorateId);
                if (userGovernorate == null) throw new ArgumentException($"User Governorate with ID {createDto.UserGovernorateId} not found.");
                if (userGovernorate.CountryId != createDto.UserCountryId) throw new ArgumentException($"User Country with ID {createDto.UserCountryId} does not match the selected User Governorate.");


                if (await _userManager.FindByEmailAsync(createDto.Email) != null) throw new InvalidOperationException($"Email '{createDto.Email}' is already registered.");

               
                string licenseUrl = null;
                string licenseId = null;
                if (licenseDocument != null)
                {
                    var uploadResult = await _fileStorageService.UploadSingleFileAsync(licenseDocument);
                    if (uploadResult.Success) { licenseUrl = uploadResult.Url; licenseId = uploadResult.Id; }
                    else { throw new InvalidOperationException($"Failed to upload driver license document: {uploadResult.Message}"); }
                }
                else { throw new ArgumentException("Driver license document is required."); } // Enforce requirement

                // 5. Handle Profile Image Upload (single file)
                string profileImageUrl = null;
                string profileImageId = null;
                if (profileImage != null)
                {
                    var uploadResult = await _fileStorageService.UploadSingleFileAsync(profileImage);
                    if (uploadResult.Success) { profileImageUrl = uploadResult.Url; profileImageId = uploadResult.Id; }
                    else { _logger.LogWarning($"Failed to upload profile image for {createDto.Email}. Proceeding without profile image. Details: {uploadResult.Message}"); }
                }

                // 6. Map DTO to Driver entity
                var driver = _mapper.Map<Driver>(createDto);
                driver.PhoneNumber = createDto.Phone;
                driver.DriveLicenseLicenseNumberURL = licenseUrl;
                driver.DriveLicenseLicenseNumberId = licenseId;
                driver.ImageURL = profileImageUrl; 
                driver.ImageId = profileImageId; 
                driver.CarRentalAssetId = carRentalAssetId; 

                // Set Identity-specific initial states
                driver.EmailConfirmed = true;
                driver.PhoneNumberConfirmed = true;
                driver.Status = Status.Active;
                driver.UserType = UserType.Driver;
                

                // 7. Create user with IdentityUserManager
                var result = await _userManager.CreateAsync(driver, createDto.Password);
                if (!result.Succeeded) { throw new InvalidOperationException($"Failed to create driver account: {string.Join(", ", result.Errors.Select(e => e.Description))}"); }

                // 8. Assign "Driver" role
                await _userManager.AddToRoleAsync(driver, "Driver");

                await _unitOfWork.CompleteAsync(); // Save additional fields (like license info)

                _logger.LogInformation($"Driver '{driver.FirstName} {driver.LastName}' (ID: {driver.Id}) created successfully for Car Rental Asset {driver.CarRentalAssetId}.");

                // 9. Re-fetch for comprehensive response DTO.
                var createdDriverWithDetails = await _unitOfWork.Drivers.GetDriverByIdWithDetailsAsync(driver.Id);
                if (createdDriverWithDetails == null) throw new Exception("Created driver not found after save.");

                return _mapper.Map<DriverResponseDto>(createdDriverWithDetails);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error creating driver for Car Rental Asset {carRentalAssetId}."); throw; }
        }

        public async Task<DriverResponseDto> DeleteDriverAsync(string driverId, string carRentalAssetId)
        {
            try
            {
                var driver = await _unitOfWork.Drivers.GetDriverByIdWithDetailsAsync(driverId);
                if (driver == null) throw new KeyNotFoundException($"Driver with ID {driverId} not found for deletion.");
                if (driver.CarRentalAssetId != carRentalAssetId) throw new UnauthorizedAccessException($"Driver with ID {driverId} is not affiliated with your Car Rental Asset.");
                if (driver.Status == Status.Deactivated) { _logger.LogInformation($"Driver {driverId} is already deactivated. No action needed."); return _mapper.Map<DriverResponseDto>(driver); }

                // Check for active assignments before deletion
                var currentAssignment = await _unitOfWork.Drivers.GetCurrentCarAssignmentForDriverAsync(driverId);
                if (currentAssignment != null)
                {
                    throw new InvalidOperationException($"Cannot delete driver {driverId} because they have an active car assignment to Car ID {currentAssignment.CarId}. Release them first.");
                }

                driver.Status = Status.Deactivated;
                var userUpdateResult = await _userManager.UpdateAsync(driver);
                if (!userUpdateResult.Succeeded) throw new InvalidOperationException($"Failed to deactivate driver account: {string.Join(", ", userUpdateResult.Errors.Select(e => e.Description))}");

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Driver '{driver.FirstName} {driver.LastName}' (ID: {driver.Id}) soft-deleted successfully for Car Rental Asset {carRentalAssetId}.");
                return _mapper.Map<DriverResponseDto>(driver);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error deleting driver with ID: {driverId} for Car Rental Asset {carRentalAssetId}."); throw; }
        }

        public async Task<PagedResponseDto<DriverResponseDto>> GetAllDriversForCarRentalAdminAsync(string carRentalAssetId, PaginationParameters paginationParameters)
        {
            try
            {
                var drivers= await _unitOfWork.Drivers.GetDriversByCarRentalAssetIdAsync(carRentalAssetId, driverStatus: null);
                IQueryable<Driver> query = drivers.AsQueryable();

                if(!string.IsNullOrWhiteSpace(paginationParameters.SearchTerm))
                {
                    string term = paginationParameters.SearchTerm.Trim().ToLower();
                    query = query.Where(s=>s.FirstName.ToLower().Contains(term)||
                                        s.LastName.ToLower().Contains(term)||
                                        s.Email.ToLower().Contains(term));
                }

                if(paginationParameters.DriverStatus.HasValue)
                {
                    query = query.Where(s => s.DriverStatus == paginationParameters.DriverStatus.Value);
                }
          

                var totalCount = query.Count();

                var pagedDrivers =  query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToList();
                var driverDtos = _mapper.Map<IEnumerable<DriverResponseDto>>(pagedDrivers);

                return new PagedResponseDto<DriverResponseDto>(driverDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);



            }
            catch (Exception ex) { _logger.LogError(ex, $"Error getting all drivers for Car Rental Admin Dashboard for Asset ID: {carRentalAssetId}."); throw; }
        }



        public async Task<DriverResponseDto?> GetDriverByIdAsync(string driverId)
        {
            try
            {
               
                var driver = await _unitOfWork.Drivers.GetDriverByIdWithDetailsAsync(driverId);

                if (driver == null)
                {
                    _logger.LogInformation($"Driver with ID {driverId} not found.");
                    return null;
                }


                return _mapper.Map<DriverResponseDto>(driver);
            }
            catch (Exception ex)
            {
                // Logs any errors that occur during the retrieval process.
                _logger.LogError(ex, $"Error getting driver by ID: {driverId}.");
                throw; // Re-throw the exception for handling by the caller (e.g., controller).
            }
        }

        public async Task<DriverResponseDto> UpdateDriverAsync(string driverId, DriverUpdateDto updateDto, string carRentalAssetId, IFormFile? newLicenseDocument = null, IFormFile? newProfileImage = null)
        {
            try
            {
                // 1. Retrieve the existing driver with details.
                var driver = await _unitOfWork.Drivers.GetDriverByIdWithDetailsAsync(driverId);
                if (driver == null) throw new KeyNotFoundException($"Driver with ID {driverId} not found for update.");

                // 2. Validate driver's affiliation.
                if (driver.CarRentalAssetId != carRentalAssetId) throw new UnauthorizedAccessException($"Driver with ID {driverId} is not affiliated with your Car Rental Asset.");

                // 3. Handle User-level property updates (Email, Phone, Status) via UserManager
                if (!string.Equals(driver.Email, updateDto.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var existingUserWithNewEmail = await _userManager.FindByEmailAsync(updateDto.Email);
                    if (existingUserWithNewEmail != null && existingUserWithNewEmail.Id != driver.Id) throw new InvalidOperationException($"Email '{updateDto.Email}' is already registered to another user.");
                    driver.Email = updateDto.Email; driver.UserName = updateDto.Email;
                }
                driver.PhoneNumber = updateDto.PhoneNumber ?? driver.PhoneNumber;
                driver.Status = updateDto.Status ?? driver.Status;

                // 4. Handle License Document Update
                if (newLicenseDocument != null)
                {
                    if (!string.IsNullOrEmpty(driver.DriveLicenseLicenseNumberId)) // Delete old file from storage if exists
                    {
                        var deleteResult = await _fileStorageService.DeleteFileAsync(driver.DriveLicenseLicenseNumberId);
                        if (!deleteResult) _logger.LogWarning($"Failed to delete old license document {driver.DriveLicenseLicenseNumberId} for driver {driverId}.");
                    }
                    var uploadResult = await _fileStorageService.UploadSingleFileAsync(newLicenseDocument, $"drivers/licenses/{driver.Email.Replace("@", "_").Replace(".", "_")}");
                    if (uploadResult.Success) { driver.DriveLicenseLicenseNumberURL = uploadResult.Url; driver.DriveLicenseLicenseNumberId = uploadResult.Id; }
                    else { throw new InvalidOperationException($"Failed to upload new license document: {uploadResult.Message}"); }
                }


                // 5. Handle Profile Image Update
                if (newProfileImage != null)
                {
                    if (!string.IsNullOrEmpty(driver.ImageId)) // Delete old file from storage if exists
                    {
                        var deleteResult = await _fileStorageService.DeleteFileAsync(driver.ImageId);
                        if (!deleteResult) _logger.LogWarning($"Failed to delete old profile image {driver.ImageId} for driver {driverId}.");
                    }
                    var uploadResult = await _fileStorageService.UploadSingleFileAsync(newProfileImage, $"drivers/profiles/{driver.Email.Replace("@", "_").Replace(".", "_")}");
                    if (uploadResult.Success) { driver.ImageURL = uploadResult.Url; driver.ImageId = uploadResult.Id; }
                    else { _logger.LogWarning($"Failed to upload new profile image for driver {driverId}. Proceeding without image. Details: {uploadResult.Message}"); }
                }


                // 6. Map remaining DTO properties (YearsOfExperience, Rating, DriverStatus, etc.)
                _mapper.Map(updateDto, driver);

                var userUpdateResult = await _userManager.UpdateAsync(driver);
                if (!userUpdateResult.Succeeded) throw new InvalidOperationException($"Failed to update driver account: {string.Join(", ", userUpdateResult.Errors.Select(e => e.Description))}");

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Driver '{driver.FirstName} {driver.LastName}' (ID: {driver.Id}) updated successfully for Car Rental Asset {driver.CarRentalAssetId}.");

                var updatedDriverWithDetails = await _unitOfWork.Drivers.GetDriverByIdWithDetailsAsync(driver.Id);
                if (updatedDriverWithDetails == null) throw new Exception("Updated driver not found after save.");

                return _mapper.Map<DriverResponseDto>(updatedDriverWithDetails);
            }
            catch (Exception ex) { _logger.LogError(ex, $"Error updating driver ID: {driverId} for Car Rental Asset {carRentalAssetId}."); throw; }
        }
    }
}
