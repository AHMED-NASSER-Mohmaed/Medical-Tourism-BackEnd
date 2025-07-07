using Elagy.Core.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public  interface IServiceProvidersWebsiteService
    {
        Task<PagedResponseDto<HotelProviderProfileDto>> GetHotelProvidersForAdminDashboardAsync(PaginationParameters paginationParameters);
        Task<PagedResponseDto<HospitalProviderProfileDto>> GetHospitalProvidersForAdminDashboardAsync(PaginationParameters paginationParameters);
        Task<PagedResponseDto<CarRentalProviderProfileDto>> GetCarRentalProvidersForAdminDashboardAsync(PaginationParameters paginationParameters);
    }
}
