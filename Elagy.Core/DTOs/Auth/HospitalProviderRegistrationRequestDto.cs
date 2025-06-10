using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Auth
{
    public class HospitalProviderRegistrationRequestDto : BaseServiceProviderRegistrationRequestDto
    {
        public int NumberOfDepartments { get; set; }
        public bool HasEmergencyRoom { get; set; }
        public bool IsTeachingHospital { get; set; }
        public bool EmergencyServices { get; set; }

        public HospitalProviderRegistrationRequestDto()
        {
            AssetType = Enums.AssetType.Hospital; // Default for this DTO
        }
    }
}