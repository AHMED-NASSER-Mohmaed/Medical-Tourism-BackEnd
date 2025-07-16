using AutoMapper;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.Helpers;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
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
    public class ServiceProvidersWebsiteService : IServiceProvidersWebsiteService
    {
       
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public ServiceProvidersWebsiteService(IUnitOfWork unitOfWork, IMapper mapper)
        {

            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        private IQueryable<ServiceProvider> ApplyServiceProviderFilters(IQueryable<ServiceProvider> query, string? searchQuery, Status? userStatus)
        {
            // Ensure ServiceAsset is included for filtering on its properties, and for later mapping
            query = query.Include(sp => sp.ServiceAsset);

            // --- 1. Apply UserStatus Filter (Independent of search query) ---
            if (userStatus.HasValue)
            {
                query = query.Where(sp => sp.Status == userStatus.Value);
            }

            // --- 2. Apply Search Term Filter ---
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string lowerSearchQuery = searchQuery.Trim().ToLower(); // Trim any whitespace


                query = query.Where(sp =>
                                        sp.Email.ToLower().Contains(lowerSearchQuery) ||
                                        sp.FirstName.ToLower().Contains(lowerSearchQuery) ||
                                        sp.LastName.ToLower().Contains(lowerSearchQuery) ||
                                        sp.Phone.ToLower().Contains(lowerSearchQuery) ||
                                        sp.ServiceAsset.Name.ToLower().Contains(lowerSearchQuery)
                                   );
            }


            return query;
        }

        public async Task<PagedResponseDto<CarRentalProviderProfileDto>> GetCarRentalProvidersForAdminDashboardAsync(PaginationParameters paginationParameters)
        {
            // 1. Start with the base query for ServiceProviders
            IQueryable<ServiceProvider> query = _unitOfWork.ServiceProviders
                .AsQueryable()
                .OfType<ServiceProvider>().Where(s=>s.Status==Status.Active); // Ensures we're working with ServiceProvider entities

            // 2. Apply the fixed filter for CarRental assets
            //    Include ServiceAsset first, as it's needed for this filter (and likely for mapping).
            query = query
                .Include(sp => sp.ServiceAsset)
                .ThenInclude(asset => ((CarRentalAsset)asset).CarRentalAssetImages).Include(s => s.ServiceAsset).ThenInclude(a => a.Governate)
                .ThenInclude(g => g.Country)  // Eager load ServiceAsset if needed for AssetType filter and mapping
                .Where(sp => sp.ServiceAsset != null && sp.ServiceAsset.AssetType == AssetType.CarRental); // Filter for CarRental assets

            if (paginationParameters.FilterGovernorateId.HasValue)
            {
                query = query.Where(sp => sp.ServiceAsset.GovernateId == paginationParameters.FilterGovernorateId.Value);
            }

            // 3. Apply additional dynamic filters (searchQuery, userStatus) using the helper function
            query = ApplyServiceProviderFilters(query, paginationParameters.SearchTerm, paginationParameters.UserStatus);

            // --- CRITICAL STEP: Get the total count AFTER all filters are applied ---
            var totalCount = await query.CountAsync();

            // 4. Apply pagination (Skip and Take)
            var pagedServiceProviders = await query
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync(); // This executes the query for the current page

            // 5. Map the results to DTOs
            // Make sure your AutoMapper configuration maps ServiceProvider to CarRentalProviderProfileDto.
            var MappedResult = _mapper.Map<List<CarRentalProviderProfileDto>>(pagedServiceProviders);

            // 6. Return the PagedResponseDto, which calculates TotalPages internally
            return new PagedResponseDto<CarRentalProviderProfileDto>(
                MappedResult,
                totalCount,
                paginationParameters.PageNumber,
                paginationParameters.PageSize
            );
        }

        public async Task<PagedResponseDto<HospitalProviderProfileDto>> GetHospitalProvidersForAdminDashboardAsync(PaginationParameters paginationParameters)
        {

            IQueryable<ServiceProvider> query = _unitOfWork.ServiceProviders
                                                .AsQueryable()
                                                .OfType<ServiceProvider>().Where(s=>s.Status==Status.Active);

            query = query.Include(sp => sp.Governorate)
                .ThenInclude(g => g.Country);

            query = query.Include(sp => sp.ServiceAsset).ThenInclude(asset => ((HospitalAsset)asset).HospitalAssetImages).Include(s => s.ServiceAsset)
                .ThenInclude(a => a.Governate)
                .ThenInclude(g => g.Country)
                .Where(sp => sp.ServiceAsset != null && sp.ServiceAsset.AssetType == AssetType.Hospital);


            if (paginationParameters.SpecialtyId.HasValue)
            {
                query = query.Where(sp =>
                   ((HospitalAsset)sp.ServiceAsset)
                   .HospitalSpecialties.Any(hs => hs.SpecialtyId == paginationParameters.SpecialtyId.Value)
               );
            }
            
            if (paginationParameters.FilterGovernorateId.HasValue)
            {
                query = query.Where(sp => sp.ServiceAsset.GovernateId == paginationParameters.FilterGovernorateId.Value);
            }

            query = ApplyServiceProviderFilters(query, paginationParameters.SearchTerm, paginationParameters.UserStatus);

            var totalCount = await query.CountAsync();


            var pagedQuery = await query
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync();


            var MappedResult = _mapper.Map<List<HospitalProviderProfileDto>>(pagedQuery);

            return new PagedResponseDto<HospitalProviderProfileDto>(
               MappedResult,
               totalCount,
               paginationParameters.PageNumber,
               paginationParameters.PageSize
           );
        }

        public async Task<PagedResponseDto<HotelProviderProfileDto>> GetHotelProvidersForAdminDashboardAsync(PaginationParameters paginationParameters)
        {
            var query = _unitOfWork.ServiceProviders.AsQueryable().OfType<ServiceProvider>().Where(s => s.Status == Status.Active);


            query = query.Include(sp => sp.ServiceAsset).ThenInclude(asset => ((HotelAsset)asset).HotelAssetImages); ;

            query = query.Include(sp => sp.Governorate)
             .ThenInclude(g => g.Country);

            query = query.Include(sp => sp.ServiceAsset)
            .ThenInclude(a => a.Governate)
            .ThenInclude(g => g.Country);


            query = query.Where(sp => sp.ServiceAsset.AssetType == AssetType.Hotel);

            if (paginationParameters.FilterGovernorateId.HasValue)
            {
                query = query.Where(s => s.ServiceAsset.GovernateId == paginationParameters.FilterGovernorateId.Value);
            }
            query = ApplyServiceProviderFilters(query, paginationParameters.SearchTerm, userStatus: paginationParameters.UserStatus);

            // --- CRITICAL STEP: Get the total count AFTER all filters are applied ---
            var totalCount = await query.CountAsync();

            // 4. Apply pagination (Skip and Take)
            var pagedServiceProviders = await query
                .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                .Take(paginationParameters.PageSize)
                .ToListAsync(); // This executes the query for the current page

            // 5. Map the results to DTOs
            // Make sure your AutoMapper configuration maps ServiceProvider to HotelProviderProfileDto.
            var MappedResult = _mapper.Map<List<HotelProviderProfileDto>>(pagedServiceProviders);

            // 6. Return the PagedResponseDto, which calculates TotalPages internally
            return new PagedResponseDto<HotelProviderProfileDto>(
                MappedResult,
                totalCount,
                paginationParameters.PageNumber,
                paginationParameters.PageSize
            );
        }
    }
}
