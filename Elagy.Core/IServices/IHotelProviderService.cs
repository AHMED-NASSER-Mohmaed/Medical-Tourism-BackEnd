using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.SpecialtyAppointment;
using Elagy.Core.DTOs.User;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IHotelProviderService
    {
        // Profile management
        Task<HotelProviderProfileDto> GetHotelProviderProfileAsync(string providerId);
        Task<HotelProviderProfileDto> UpdateHotelProviderProfileAsync(string providerId, HotelProviderProfileUpdateDto model);

        // For Super Admin to add providers directly
        Task<List<AssetImageResponseDto>> UploadHotelAssetImages(string hotelId, List<IFormFile> HotelImages);
        Task<AuthResultDto> AddHotelProviderByAdminAsync(HotelAssetRegistrationRequestDto model);
        Task<List<AssetImageResponseDto>> DeleteHotelAssetImagesByIds(string hotelId, List<string> imageIds);
        Task<PagedResponseDto<HospitalAppointmentDto>> GetHospitalAppointmentsAsync( string hospitalAssetId,PaginationParameters paginationParameters);
    }
}