using AutoMapper;
using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.Helpers;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Crucial for .Include() and .SingleOrDefaultAsync()
using ServiceProvider = Elagy.Core.Entities.ServiceProvider; // Ensure this is the correct namespace for ServiceProvider

using Microsoft.Extensions.Logging; // Add this line if it's missing
namespace Elagy.BL.Services
{
    public class CarRentalProviderService : ICarRentalProviderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CarRentalProviderService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public CarRentalProviderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CarRentalProviderService> logger,
                                        UserManager<User> userManager, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<CarRentalProviderProfileDto> GetCarRentalProviderProfileAsync(string providerId)
        {
            // Corrected query: Ensure Include is called directly on the IQueryable from AsQueryable()
            var provider = await _unitOfWork.ServiceProviders.AsQueryable()
                                            .Include(sp => sp.ServiceAsset) 
                                            .SingleOrDefaultAsync(sp => sp.Id == providerId);

            if (provider == null || provider.ServiceAsset == null || provider.ServiceAsset.AssetType != AssetType.CarRental)
            {
                _logger.LogWarning($"Car Rental Provider with ID {providerId} or its asset not found/invalid type.");
                return null;
            }

            return _mapper.Map<CarRentalProviderProfileDto>(provider);
        }

        public async Task<CarRentalProviderProfileDto> UpdateCarRentalProviderProfileAsync(string providerId, CarRentalProviderProfileUpdateDto model)
        {
            // Corrected query:
            var provider = await _unitOfWork.ServiceProviders.AsQueryable()
                                            .Include(sp => sp.ServiceAsset)
                                            .SingleOrDefaultAsync(sp => sp.Id == providerId);

            if (provider == null || provider.ServiceAsset == null || provider.ServiceAsset.AssetType != AssetType.CarRental)
            {
                _logger.LogWarning($"Failed to update: Car Rental Provider with ID {providerId} or its asset not found/invalid type.");
                return null;
            }

            var carRentalAsset = provider.ServiceAsset as CarRentalAsset;
            if (carRentalAsset == null)
            {
                _logger.LogError($"Asset for provider {providerId} is not a CarRentalAsset during update.");
                return null;
            }

            // Map common user (ServiceProvider) properties
            _mapper.Map(model, provider);
            // Map common ServiceAsset properties and specific CarRentalAsset properties
            _mapper.Map(model, carRentalAsset);

            _unitOfWork.ServiceProviders.Update(provider); // Mark provider for update
            _unitOfWork.CarRentalAssets.Update(carRentalAsset); // Mark asset for update

            await _unitOfWork.CompleteAsync(); // Save changes

            _logger.LogInformation($"Car Rental Provider with ID {providerId} profile updated successfully.");
            return _mapper.Map<CarRentalProviderProfileDto>(provider); // Return updated DTO
        }

        public async Task<AuthResultDto> AddCarRentalProviderByAdminAsync(CarRentalProviderRegistrationRequestDto model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Email already registered." } };
            }

            var serviceProvider = _mapper.Map<ServiceProvider>(model);
            serviceProvider.UserType = UserType.ServiceProvider;
            serviceProvider.Status = UserStatus.Active; // Admin adds directly to active
            serviceProvider.EmailConfirmed = true; // Admin adds directly to confirmed

            var result = await _userManager.CreateAsync(serviceProvider, model.Password);

            if (!result.Succeeded)
            {
                _logger.LogError($"Admin Car Rental Provider creation failed for {serviceProvider.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }

            var carRentalAsset = _mapper.Map<CarRentalAsset>(model);
            carRentalAsset.Id = serviceProvider.Id; // Set AssetId to match ServiceProvider's Id
            carRentalAsset.ServiceProvider = serviceProvider; // Link to the provider
            carRentalAsset.AssetType = AssetType.CarRental;
            carRentalAsset.AcquisitionDate = DateTime.UtcNow; // Set creation date
            carRentalAsset.VerificationStatus = VerificationStatus.Verified; // Admin adds directly to verified

            try
            {
                await _unitOfWork.CarRentalAssets.AddAsync(carRentalAsset);
                await _unitOfWork.CompleteAsync(); // Save the asset and link
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Failed to create CarRentalAsset for {serviceProvider.Email}. Rolling back user creation.");
                await _userManager.DeleteAsync(serviceProvider); // Rollback user creation
                return new AuthResultDto { Success = false, Errors = new[] { "Failed to create associated asset. Please try again." } };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error creating CarRentalAsset for {serviceProvider.Email}. Rolling back user creation.");
                await _userManager.DeleteAsync(serviceProvider); // Rollback user creation
                return new AuthResultDto { Success = false, Errors = new[] { "An unexpected error occurred during asset creation." } };
            }

            _logger.LogInformation($"Car Rental Provider {serviceProvider.Email} added by admin with asset.");
            return new AuthResultDto { Success = true, Message = "Car Rental Provider account and asset created successfully by admin." };
        }
    }
}