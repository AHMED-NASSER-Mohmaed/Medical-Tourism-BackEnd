using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Auth
{


    public class HospitalAssetRegistrationRequestDto : BaseAssetRegistrationRequestDto
    {
        [Required(ErrorMessage = "Please provide the number of departments in the hospital.")]
        public int NumberOfDepartments { get; set; }
        public bool EmergencyServices { get; set; }

        public HospitalAssetRegistrationRequestDto()
        {
            AssetType = Enums.AssetType.Hospital; // Default for this DTO
        }


    }
}