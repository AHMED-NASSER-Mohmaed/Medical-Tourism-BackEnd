using AutoMapper;
using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.Helpers;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http; // Add this line if it's missing
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Crucial for .Include() and .SingleOrDefaultAsync()
using Microsoft.Extensions.Logging;
using ServiceProvider = Elagy.Core.Entities.ServiceProvider; // Ensure this is the correct namespace for ServiceProvider

namespace Elagy.BL.Services
{
    public class HotelProviderService : IHotelProviderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HotelProviderService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IFileStorageService _filestorageservice;


        public HotelProviderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HotelProviderService> logger,
                                    UserManager<User> userManager, IEmailService emailService, IFileStorageService filestorageservice)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _emailService = emailService;
            _filestorageservice = filestorageservice;
        }

        public async Task<HotelProviderProfileDto> GetHotelProviderProfileAsync(string providerId)
        {
            // Corrected query: Ensure Include is called directly on the IQueryable from AsQueryable()
            var provider = await _unitOfWork.ServiceProviders.AsQueryable()
                                            .Include(sp => sp.ServiceAsset) // This should now work
                                            .SingleOrDefaultAsync(sp => sp.Id == providerId);

            if (provider == null || provider.ServiceAsset == null || provider.ServiceAsset.AssetType != AssetType.Hotel)
            {
                _logger.LogWarning($"Hotel Provider with ID {providerId} or its asset not found/invalid type.");
                return null;
            }

            return _mapper.Map<HotelProviderProfileDto>(provider);
        }

        public async Task<HotelProviderProfileDto> UpdateHotelProviderProfileAsync(string providerId, HotelProviderProfileUpdateDto model)
        {
            // Corrected query:
            var provider = await _unitOfWork.ServiceProviders.AsQueryable()
                                            .Include(sp => sp.ServiceAsset)
                                            .SingleOrDefaultAsync(sp => sp.Id == providerId);

            if (provider == null || provider.ServiceAsset == null || provider.ServiceAsset.AssetType != AssetType.Hotel)
            {
                _logger.LogWarning($"Failed to update: Hotel Provider with ID {providerId} or its asset not found/invalid type.");
                return null;
            }

            var hotelAsset = provider.ServiceAsset as HotelAsset;
            if (hotelAsset == null)
            {
                _logger.LogError($"Asset for provider {providerId} is not a HotelAsset during update.");
                return null;
            }

            // Map common user (ServiceProvider) properties
            _mapper.Map(model, provider);
            // Map common ServiceAsset properties and specific HotelAsset properties
            _mapper.Map(model, hotelAsset);

            _unitOfWork.ServiceProviders.Update(provider); // Mark provider for update
            _unitOfWork.HotelAssets.Update(hotelAsset); // Mark asset for update

            await _unitOfWork.CompleteAsync(); // Save changes

            _logger.LogInformation($"Hotel Provider with ID {providerId} profile updated successfully.");
            return _mapper.Map<HotelProviderProfileDto>(provider); // Return updated DTO
        }

        public async Task<AuthResultDto> AddHotelProviderByAdminAsync(HotelAssetRegistrationRequestDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Email already registered." } };
            }

            var serviceProvider = _mapper.Map<ServiceProvider>(model);
            serviceProvider.UserType = UserType.ServiceProvider;
            serviceProvider.Status = Status.Active; // Admin adds directly to active
            serviceProvider.EmailConfirmed = true; // Admin adds directly to confirmed

            var result = await _userManager.CreateAsync(serviceProvider, model.Password);

            if (!result.Succeeded)
            {
                _logger.LogError($"Admin Hotel Provider creation failed for {serviceProvider.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }

            var hotelAsset = _mapper.Map<HotelAsset>(model);
            hotelAsset.Id = serviceProvider.Id; // Set AssetId to match ServiceProvider's Id
            hotelAsset.ServiceProvider = serviceProvider; // Link to the provider
            hotelAsset.AssetType = AssetType.Hotel;
 
            try
            {
                await _unitOfWork.HotelAssets.AddAsync(hotelAsset);
                await _unitOfWork.CompleteAsync(); // Save the asset and link
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Failed to create HotelAsset for {serviceProvider.Email}. Rolling back user creation.");
                await _userManager.DeleteAsync(serviceProvider); // Rollback user creation
                return new AuthResultDto { Success = false, Errors = new[] { "Failed to create associated asset. Please try again." } };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error creating HotelAsset for {serviceProvider.Email}. Rolling back user creation.");
                await _userManager.DeleteAsync(serviceProvider); // Rollback user creation
                return new AuthResultDto { Success = false, Errors = new[] { "An unexpected error occurred during asset creation." } };
            }

            _logger.LogInformation($"Hotel Provider {serviceProvider.Email} added by admin with asset.");
            return new AuthResultDto { Success = true, Message = "Hotel Provider account and asset created successfully by admin." };
        }

        public async Task<List<AssetImageResponseDto>> UploadHotelAssetImages(string hotelId, List<IFormFile> HotelImages)
        {
            if (HotelImages == null || !HotelImages.Any())
            {
                throw new ArgumentException("No images provided for upload");
            }

  
            var hotel = await _unitOfWork.HotelAssets.GetByIdAsync(hotelId);
            if (hotel == null)
            {
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found");
            }

            var uploadResult = await _filestorageservice.UploadMultipleFilesAsync(HotelImages);


            if (!uploadResult.OverallSuccess)
            {
                var errorMessages = uploadResult.UploadResults
                    .Where(r => !r.Success)
                    .Select(r => r.Error);
                throw new ApplicationException($"Image upload failed: {string.Join(", ", errorMessages)}");
            }

            var images = new List<HotelAssetImage>();
            foreach (var result in uploadResult.UploadResults.Where(r => r.Success))
            {
                images.Add(new HotelAssetImage
                {
                    ImageId = result.Id,
                    ImageURL = result.Url, 
                    HotelAssetId = hotelId 
                });
            }

            await _unitOfWork.HotelAssetImages.AddRangeAsync(images);
            await _unitOfWork.CompleteAsync();

            var imagesResponse = _mapper.Map<List<AssetImageResponseDto>>(images);
            return imagesResponse;
        }

        public async Task<List<AssetImageResponseDto>> DeleteHotelAssetImagesByIds(string hotelId, List<string> imageIds)
        {
            if (imageIds == null || !imageIds.Any())
                throw new ArgumentException("No image IDs provided.");

            var hotel = await _unitOfWork.HotelAssets.GetByIdAsync(hotelId);
            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {hotelId} not found");

            var imagesToDelete = await _unitOfWork.HotelAssetImages
                .FindAsync(img => imageIds.Contains(img.ImageId) && img.HotelAssetId == hotelId.ToString());

            if (!imagesToDelete.Any())
                throw new KeyNotFoundException("No matching images found for deletion.");

            var imageUrls = imagesToDelete.Select(img => img.ImageURL).ToList();
            var deleteResult = await _filestorageservice.DeleteMultipleFilesAsync(imageUrls);

            if (!deleteResult.OverallSuccess)
            {
                var errorMessages = deleteResult.DeletionResults
                    .Where(r => !r.Success)
                    .Select(r => r.ErrorDetails);
                throw new ApplicationException($"File deletion failed: {string.Join(", ", errorMessages)}");
            }

            _unitOfWork.HotelAssetImages.RemoveRange(imagesToDelete);
            await _unitOfWork.CompleteAsync();

            var deletedImageDtos = _mapper.Map<List<AssetImageResponseDto>>(imagesToDelete);
            return deletedImageDtos;
        }
    }
}