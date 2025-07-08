using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Disbursement;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface ICarRentalProviderService
    {
        Task<CarRentalProviderProfileDto> GetCarRentalProviderProfileAsync(string providerId);
        Task<CarRentalProviderProfileDto> UpdateCarRentalProviderProfileAsync(string providerId, CarRentalProviderProfileUpdateDto model);
        Task<AuthResultDto> AddCarRentalProviderByAdminAsync(CarRentalAssetRegistrationRequestDto model);

        Task <PagedResponseDto<DisplayCarRentalDisbursement>> GetAllDisbursement(string providerId);
        Task <DisplayCarRentalDisbursement> GetDisbursementWithDetails(int id ,string ProviderId);
    }
}