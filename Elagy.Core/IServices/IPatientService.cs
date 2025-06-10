using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IPatientService
    {
        // Profile management
        Task<PatientDto> GetPatientProfileAsync(string patientId);
        Task<PatientDto> UpdatePatientProfileAsync(string patientId, PatientProfileUpdateDto model);

        // For Super Admin to add patients directly
        Task<AuthResultDto> AddPatientByAdminAsync(PatientRegistrationRequestDto model);
    }
}