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
using Microsoft.Extensions.Logging;
using Elagy.Core.DTOs.Pagination; // Add this line if it's missing
namespace Elagy.BL.Services
{
    public class HospitalProviderService : IHospitalProviderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<HospitalProviderService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public HospitalProviderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<HospitalProviderService> logger,
                                       UserManager<User> userManager, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _emailService = emailService;
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

        public async Task<PagedResponseDto<HospitalProviderProfileDto>> GetHospitalsForWebsiteAsync(PaginationParameters paginationParameters)
        {
              try
            {
                //Start with an IQueryable of ServiceProviders and eager load necessary details.
                IQueryable<HospitalAsset> query = _unitOfWork.HospitalAssets.AsQueryable()
                    .Include(ha => ha.HospitalSpecialties) 
                        .ThenInclude(hs => hs.Specialty) 
                    .Include(ha => ha.ServiceProvider) 
                        .ThenInclude(sp => sp.Governorate) 
                            .ThenInclude(g => g.Country);

                if (paginationParameters.SpecialtyId.HasValue)
                {
                    // Filter based on HospitalSpecialties collection
                    query = query.Where(ha =>
                        ha.HospitalSpecialties != null &&
                        ha.HospitalSpecialties.Any(hs => hs.SpecialtyId == paginationParameters.SpecialtyId.Value)
                    );
                }

                if (paginationParameters.FilterGovernorateId.HasValue)
                {
                    // Access GovernorateId through the ServiceProvider navigation property
                    query = query.Where(ha => ha.ServiceProvider.GovernorateId == paginationParameters.FilterGovernorateId.Value);
                }

                // Search by hospital name, city, address
                if (!string.IsNullOrWhiteSpace(paginationParameters.SearchTerm))
                {
                    string term = paginationParameters.SearchTerm.Trim().ToLower();
                    query = query.Where(ha =>
                        ha.Name.ToLower().Contains(term) || 
                        (ha.ServiceProvider.City != null && ha.ServiceProvider.City.ToLower().Contains(term)) // City is on User base (ServiceProvider)
                    );
                }

                // 5. Get total count AFTER all filters are applied
                var totalCount = await query.CountAsync();

                // 6. Apply pagination (Skip and Take)
                var pagedHospitals = await query
                    .OrderBy(ha => ha.Name) 
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                // 7. Map entities to DTOs
                var hospitalDtos = _mapper.Map<IEnumerable<HospitalProviderProfileDto>>(pagedHospitals);

                return new PagedResponseDto<HospitalProviderProfileDto>(
                    hospitalDtos,
                    totalCount,
                    paginationParameters.PageNumber,
                    paginationParameters.PageSize
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting hospital providers for website.");
                return new PagedResponseDto<HospitalProviderProfileDto>(Enumerable.Empty<HospitalProviderProfileDto>(), 0, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
        }
    }
}