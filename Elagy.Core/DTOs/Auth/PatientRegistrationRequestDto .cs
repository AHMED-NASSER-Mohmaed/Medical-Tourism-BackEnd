using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Auth
{
    public class PatientRegistrationRequestDto : BaseRegistrationRequestDto
    {
        //[Required]
        public string? BloodGroup { get; set; }

        public float? Height { get; set; }
        public float? Weight { get; set; }
    }
}