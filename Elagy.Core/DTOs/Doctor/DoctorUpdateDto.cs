using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Doctor
{
    public class DoctorUpdateDto
    {
        [Required(ErrorMessage = "Doctor ID is required.")]
        public string Id { get; set; } // Doctor's IdentityUser ID

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(100)]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public string Nationality { get; set; }
        public string NationalId { get; set; }
        public string PassportId { get; set; }
        public string ImageId { get; set; }
        public string ImageURL { get; set; }
        public string Gender { get; set; }
        public UserStatus Status { get; set; } // Admin can change status (Active, Inactive, etc.)
        public string ZipCode { get; set; }
        public string StreetNumber { get; set; }
        public string Governorate { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Medical License Number is required.")]
        [StringLength(50)]
        public string MedicalLicenseNumber { get; set; }

        [Range(0, 70, ErrorMessage = "Years of experience must be between 0 and 70.")]
        public int YearsOfExperience { get; set; }

        [StringLength(1000)]
        public string Bio { get; set; }

        [StringLength(100)]
        public string Qualification { get; set; }

        // Admin might need to re-assign a doctor to a different specialty within the hospital
        [Required(ErrorMessage = "Hospital Specialty ID is required.")]
        public int HospitalSpecialtyId { get; set; }
    }
}
