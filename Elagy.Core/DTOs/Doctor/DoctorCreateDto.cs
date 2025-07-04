using Elagy.Core.DTOs.Auth;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Doctor
{
    public class DoctorCreateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = default!;

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = default!;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; } = default!;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; } = default!;

        [Required]
        public string Phone { get; set; }

        [Required]
        public Gender Gender { get; set; }

        #region
        [Required]
        public string? Address { get; set; } // Full address as a single string

        [Required]
        public string? City { get; set; }

        [Required]
        public int GovernorateId { get; set; }
        [Required]
        public int CountryId { get; set; }
        #endregion

        [Required]
        public DateTime DateOfBirth { get; set; }
        [StringLength(50)]

        public int YearsOfExperience { get; set; }
        [StringLength(1000)]
        public string? Bio { get; set; }
        [StringLength(100)]
        public string? Qualification { get; set; }
        [Required(ErrorMessage = "Specialty/Clinic is required.")]
        public int HospitalSpecialtyId { get; set; } // This would come from a dropdown populated by an API call
    }
}
