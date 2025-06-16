using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.SpecialtyDTO;
using Elagy.Core.DTOs.User;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IHospitalProviderService
    {
        Task<HospitalProviderProfileDto> GetHospitalProviderProfileAsync(string providerId);
        Task<HospitalProviderProfileDto> UpdateHospitalProviderProfileAsync(string providerId, HospitalProviderProfileUpdateDto model);
        Task<AuthResultDto> AddHospitalProviderByAdminAsync(HospitalProviderRegistrationRequestDto model);
        //Task<IEnumerable<HospitalMinDto>> GetFilteredHospitalsForWebsiteAsync(HospitalFilterDto filter);
        //Task<HospitalWebsiteDto> GetHospitalDetailsForWebsiteAsync(string hospitalId);
        //Task<IEnumerable<SpecialtyDto>> GetSpecialtiesForHospitalWebsiteAsync(string hospitalId); // List specialties in a specific hospital
        //Task<IEnumerable<HospitalMinDto>> GetHospitalsBySpecialtyForWebsiteAsync(int specialtyId); // List hospitals for a specific specialty
    }
}