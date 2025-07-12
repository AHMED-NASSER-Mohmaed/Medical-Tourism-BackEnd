using AutoMapper;
using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.SpecialtyAppointment;
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
    public class HospitalProviderService : IHospitalProviderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HospitalProviderService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly IFileStorageService _filestorageservice;


        public HospitalProviderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HospitalProviderService> logger,
                                       UserManager<User> userManager, IEmailService emailService, IFileStorageService filestorageservice)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _emailService = emailService;
            _filestorageservice = filestorageservice;
        }

        public async Task<HospitalProviderProfileDto> GetHospitalProviderProfileAsync(string providerId)
        {
            // Corrected query: Ensure Include is called directly on the IQueryable from AsQueryable()
            var provider = await _unitOfWork.ServiceProviders.AsQueryable()
                                            .Include(sp => sp.ServiceAsset) // This should now work
                                            .SingleOrDefaultAsync(sp => sp.Id == providerId);

            if (provider == null || provider.ServiceAsset == null || provider.ServiceAsset.AssetType != AssetType.Hospital)
            {
                _logger.LogWarning($"Hospital Provider with ID {providerId} or its asset not found/invalid type.");
                return null;
            }

            return _mapper.Map<HospitalProviderProfileDto>(provider);
        }

        public async Task<HospitalProviderProfileDto> UpdateHospitalProviderProfileAsync(string providerId, HospitalProviderProfileUpdateDto model)
        {
            // Corrected query:
            var provider = await _unitOfWork.ServiceProviders.AsQueryable()
                                            .Include(sp => sp.ServiceAsset)
                                            .SingleOrDefaultAsync(sp => sp.Id == providerId);

            if (provider == null || provider.ServiceAsset == null || provider.ServiceAsset.AssetType != AssetType.Hospital)
            {
                _logger.LogWarning($"Failed to update: Hospital Provider with ID {providerId} or its asset not found/invalid type.");
                return null;
            }

            var hospitalAsset = provider.ServiceAsset as HospitalAsset;
            if (hospitalAsset == null)
            {
                _logger.LogError($"Asset for provider {providerId} is not a HospitalAsset during update.");
                return null;
            }

            // Map common user (ServiceProvider) properties
            _mapper.Map(model, provider);
            // Map common ServiceAsset properties and specific HospitalAsset properties
            _mapper.Map(model, hospitalAsset);

            _unitOfWork.ServiceProviders.Update(provider); // Mark provider for update
            _unitOfWork.HospitalAssets.Update(hospitalAsset); // Mark asset for update

            await _unitOfWork.CompleteAsync(); // Save changes

            _logger.LogInformation($"Hospital Provider with ID {providerId} profile updated successfully.");
            return _mapper.Map<HospitalProviderProfileDto>(provider); // Return updated DTO
        }

        public async Task<AuthResultDto> AddHospitalProviderByAdminAsync(HospitalAssetRegistrationRequestDto model)
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
                _logger.LogError($"Admin Hospital Provider creation failed for {serviceProvider.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }

            var hospitalAsset = _mapper.Map<HospitalAsset>(model);
            hospitalAsset.Id = serviceProvider.Id; // Set AssetId to match ServiceProvider's Id
            hospitalAsset.ServiceProvider = serviceProvider; // Link to the provider
            hospitalAsset.AssetType = AssetType.Hospital;
 
            try
            {
                await _unitOfWork.HospitalAssets.AddAsync(hospitalAsset);
                await _unitOfWork.CompleteAsync(); // Save the asset and link
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Failed to create HospitalAsset for {serviceProvider.Email}. Rolling back user creation.");
                await _userManager.DeleteAsync(serviceProvider); // Rollback user creation
                return new AuthResultDto { Success = false, Errors = new[] { "Failed to create associated asset. Please try again." } };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error creating HospitalAsset for {serviceProvider.Email}. Rolling back user creation.");
                await _userManager.DeleteAsync(serviceProvider); // Rollback user creation
                return new AuthResultDto { Success = false, Errors = new[] { "An unexpected error occurred during asset creation." } };
            }

            _logger.LogInformation($"Hospital Provider {serviceProvider.Email} added by admin with asset.");
            return new AuthResultDto { Success = true, Message = "Hospital Provider account and asset created successfully by admin." };
        }

        public async Task<List<AssetImageResponseDto>> UploadHospitalAssetImages(string hospitalId, List<IFormFile> hospitalImages)
        {
            if (hospitalImages == null || !hospitalImages.Any())
                throw new ArgumentException("No images provided for upload");

            var hospital = await _unitOfWork.HospitalAssets.GetByIdAsync(hospitalId);
            if (hospital == null)
                throw new KeyNotFoundException($"Hospital with ID {hospitalId} not found");

            var uploadResult = await _filestorageservice.UploadMultipleFilesAsync(hospitalImages);

            if (!uploadResult.OverallSuccess)
            {
                var errorMessages = uploadResult.UploadResults
                    .Where(r => !r.Success)
                    .Select(r => r.Error);
                throw new ApplicationException($"Image upload failed: {string.Join(", ", errorMessages)}");
            }

            var images = uploadResult.UploadResults
                .Where(r => r.Success)
                .Select(r => new HospitalAssetImage
                {
                    ImageId = r.Id,
                    ImageURL = r.Url,
                    HospitalAssetId = hospitalId
                }).ToList();

            await _unitOfWork.HospitalAssetImages.AddRangeAsync(images);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<List<AssetImageResponseDto>>(images);
        }

        public async Task<List<AssetImageResponseDto>> DeleteHospitalAssetImagesByIds(string hospitalId, List<string> imageIds)
        {
            if (imageIds == null || !imageIds.Any())
                throw new ArgumentException("No image IDs provided.");

            var hospital = await _unitOfWork.HospitalAssets.GetByIdAsync(hospitalId);
            if (hospital == null)
                throw new KeyNotFoundException($"Hospital with ID {hospitalId} not found");

            var imagesToDelete = await _unitOfWork.HospitalAssetImages
                .FindAsync(img => imageIds.Contains(img.ImageId) && img.HospitalAssetId == hospitalId.ToString());

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

            _unitOfWork.HospitalAssetImages.RemoveRange(imagesToDelete);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<List<AssetImageResponseDto>>(imagesToDelete);
        }

        public async Task<PagedResponseDto<HospitalAppointmentDto>> GetHospitalAppointmentsAsync(
    string hospitalAssetId,
    PaginationParameters paginationParameters)
        {
            var query = _unitOfWork.SpecialtyAppointments.AsQueryable()
                .Include(a => a.Package)
                    .ThenInclude(p => p.Patient)
                        .ThenInclude(p => p.Governorate)
                            .ThenInclude(g => g.Country)
                .Include(a => a.SpecialtySchedule)
                    .ThenInclude(s => s.HospitalSpecialty)
                        .ThenInclude(hs => hs.Specialty)
                .Include(a => a.SpecialtySchedule.Doctor)
                .Where(a => a.SpecialtySchedule.HospitalSpecialty.HospitalAssetId == hospitalAssetId);

            if (paginationParameters.AppointmentStatus.HasValue)
            {
                query = query.Where(a => a.Status == paginationParameters.AppointmentStatus.Value);
            }

            if (paginationParameters.FilterStartDate.HasValue)
            {
                query = query.Where(a => a.Date == paginationParameters.FilterStartDate.Value);
            }
            if (paginationParameters.FilterDayOfWeekId.HasValue)
            {
                query = query.Where(a => a.SpecialtySchedule.DayOfWeekId == paginationParameters.FilterDayOfWeekId.Value);
            }

            var totalCount = await query.CountAsync();

            var pagedAppointments = await query
                .OrderByDescending(a => a.Date)
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync();


        }
    }
}