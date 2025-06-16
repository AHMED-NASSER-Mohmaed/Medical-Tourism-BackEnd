using Elagy.Core.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.DoctorDTO
{
    public class DoctorCreateDto:BaseRegistrationRequestDto // For the Add Doctor modal form
    {
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Medical License Number is required.")]
        [StringLength(50)]
        public string MedicalLicenseNumber { get; set; }
        [Range(0, 70, ErrorMessage = "Years of experience must be between 0 and 70.")]
        public int YearsOfExperience { get; set; }
        [StringLength(1000)]
        public string? Bio { get; set; }
        [StringLength(100)]
        public string? Qualification { get; set; }
        [Required(ErrorMessage = "Specialty/Clinic is required.")]
        public int HospitalSpecialtyId { get; set; } // This would come from a dropdown populated by an API call
    }
}
