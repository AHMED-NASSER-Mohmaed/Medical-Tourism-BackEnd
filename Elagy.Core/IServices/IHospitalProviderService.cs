using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Disbursement;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.Specialty;
using Elagy.Core.DTOs.SpecialtyAppointment;
using Elagy.Core.DTOs.User;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IHospitalProviderService
    {
        Task<HospitalProviderProfileDto> GetHospitalProviderProfileAsync(string providerId);
        Task<HospitalProviderProfileDto> UpdateHospitalProviderProfileAsync(string providerId, HospitalProviderProfileUpdateDto model);
        Task<AuthResultDto> AddHospitalProviderByAdminAsync(HospitalAssetRegistrationRequestDto model);
        Task<List<AssetImageResponseDto>> UploadHospitalAssetImages(string hospitalId, List<IFormFile> hospitalImages);
        Task<List<AssetImageResponseDto>> DeleteHospitalAssetImagesByIds(string hospitalId, List<string> imageIds);
        Task<PagedResponseDto<DisplayDisbursement>> GetDisbursement(string ProviderId, PaginationParameters paginationParams);
        Task<DisbursementHospitalDTO> GetDisbursementWithDetails(int disbursementId, string ProviderId);
        Task<PagedResponseDto<HospitalAppointmentDto>> GetHospitalAppointmentsAsync(string hospitalAssetId, PaginationParameters paginationParameters);


    }
}