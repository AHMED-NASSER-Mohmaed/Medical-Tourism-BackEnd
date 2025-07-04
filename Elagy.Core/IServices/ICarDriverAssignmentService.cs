using Elagy.Core.DTOs.Driver;
using Elagy.Core.DTOs.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface ICarDriverAssignmentService
    {

        Task<CarDriverResponseDto> AssignDriverToCarAsync(CarDriverCreateDto createDto, string carRentalAssetId);

        Task<CarDriverResponseDto?> ReleaseDriverFromCarAsync(int carId, string driverId, string carRentalAssetId);

        Task<CarDriverResponseDto?> GetCurrentAssignmentForDriverAsync(string driverId, string carRentalAssetId);

        Task<PagedResponseDto<CarDriverResponseDto>> GetAssignmentsForCarRentalAssetAsync(string carRentalAssetId, PaginationParameters paginationParameters);

        Task<PagedResponseDto<CarDriverResponseDto>> GetAssignmentHistoryForCarAsync(int carId, string carRentalAssetId, PaginationParameters paginationParameters);

        Task<PagedResponseDto<CarDriverResponseDto>> GetAssignmentHistoryForDriverAsync(string driverId, string carRentalAssetId, PaginationParameters paginationParameters);
    }
}
