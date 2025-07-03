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
        /// <summary>
        /// Assigns a specific driver to a specific car for the current car rental asset.
        Task<CarDriverResponseDto> AssignDriverToCarAsync(CarDriverCreateDto createDto, string carRentalAssetId);

        /// <summary>
        /// Releases a driver from a car, marking a CarDriver assignment as no longer current.
        Task<CarDriverResponseDto?> ReleaseDriverFromCarAsync(int carId, string driverId, string carRentalAssetId);

        /// <summary>
        /// Retrieves the current active assignment for a specific driver.
        Task<CarDriverResponseDto?> GetCurrentAssignmentForDriverAsync(string driverId, string carRentalAssetId);

        /// Retrieves all CarDriver assignments (history or current) for a specific car rental asset.
        Task<PagedResponseDto<CarDriverResponseDto>> GetAssignmentsForCarRentalAssetAsync(string carRentalAssetId, PaginationParameters paginationParameters);

        /// Retrieves assignment history for a specific car.
        Task<PagedResponseDto<CarDriverResponseDto>> GetAssignmentHistoryForCarAsync(int carId, string carRentalAssetId, PaginationParameters paginationParameters);

        /// Retrieves assignment history for a specific driver.
        Task<PagedResponseDto<CarDriverResponseDto>> GetAssignmentHistoryForDriverAsync(string driverId, string carRentalAssetId, PaginationParameters paginationParameters);
    }
}
