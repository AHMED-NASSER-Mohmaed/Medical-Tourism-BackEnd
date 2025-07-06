using Elagy.Core.DTOs.CarRentals;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.TOP;
using Elagy.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface ICarService
    {
        // --- Car Rental Admin Dashboard Car Management ---
        Task<PagedResponseDto<CarResponseDto>> GetAllCarsForCarRentalAdminAsync(string carRentalAssetId, PaginationParameters paginationParameters);
        Task<CarResponseDto> CreateCarAsync(CarCreateDto createDto, string carRentalAssetId, List<IFormFile>? imageFiles = null);
        Task<CarResponseDto> UpdateCarAsync(int carId, CarUpdateDto updateDto, string carRentalAssetId, List<IFormFile>? newImageFiles = null);
        Task<CarResponseDto> ChangeCarAvailabilityStatusAsync(int carId, bool newIsAvailableStatus, string carRentalAssetId);
        Task<CarResponseDto> ChangeCarInternalStatusAsync(int carId, CarStatus newInternalStatus, string carRentalAssetId);


        // --- Public/Website Car Display & Details ---
        Task<PagedResponseDto<CarResponseDto>> GetAvailableCarsForWebsiteAsync(PaginationParameters paginationParameters, string CarRentalAssetId);
        Task<CarResponseDto?> GetCarByIdAsync(int carId);
        Task<IEnumerable<CarBookingCountDto>> GetTopCarsByRentals();
    }
}
