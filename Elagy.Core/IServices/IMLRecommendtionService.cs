using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
   public  interface IMLRecommendtionService
    {
        Task<PagedResponseDto<HospitalProviderProfileDto>> RecommendedHospitalForUser(PaginationParameters paginationParameters, string userId);
        Task<PagedResponseDto<HotelProviderProfileDto>> RecommendetionHotelsForUser(PaginationParameters paginationParameters, string userId);
        Task<PagedResponseDto<CarRentalProviderProfileDto>> RecommendetionCarRentalForUser(PaginationParameters paginationParameters, string userId);

    }
}
