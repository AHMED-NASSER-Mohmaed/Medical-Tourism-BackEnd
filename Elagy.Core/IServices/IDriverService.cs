using Elagy.Core.DTOs.Driver;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IDriverService
    {
        // --- Car Rental Admin Dashboard Driver Management ---
        Task<PagedResponseDto<DriverResponseDto>> GetAllDriversForCarRentalAdminAsync(string carRentalAssetId, PaginationParameters paginationParameters);
        Task<DriverResponseDto> CreateDriverAsync(DriverCreateDto createDto, string carRentalAssetId, IFormFile? licenseDocument, IFormFile? profileImage);
        Task<DriverResponseDto> UpdateDriverAsync(string driverId, DriverUpdateDto updateDto, string carRentalAssetId, IFormFile? newLicenseDocument = null, IFormFile? newProfileImage = null);
        Task<DriverResponseDto> DeleteDriverAsync(string driverId, string carRentalAssetId);
        Task<DriverResponseDto> ChangeDriverInternalStatusAsync(string driverId, DriverStatus newDriverStatus, string carRentalAssetId);


        // --- Public/Website Driver Display & Details ---
        Task<DriverResponseDto?> GetDriverByIdAsync(string driverId);
       
    }
}
