using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IHospitalProviderService
    {
        Task<HospitalProviderProfileDto> GetHospitalProviderProfileAsync(string providerId);
        Task<HospitalProviderProfileDto> UpdateHospitalProviderProfileAsync(string providerId, HospitalProviderProfileUpdateDto model);
        Task<AuthResultDto> AddHospitalProviderByAdminAsync(HospitalProviderRegistrationRequestDto model);
    }
}