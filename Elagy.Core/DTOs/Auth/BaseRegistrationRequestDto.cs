using Elagy.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Auth
{
    public abstract class BaseRegistrationRequestDto
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
        public string PhoneNumber { get; set; }

        [Required]
        public string Gender { get; set; }


        #region
        [Required]
        public string Address { get; set; } // Full address as a single string

        [Required]
        public string City { get; set; } // City name, not a complex object

        [Required]
        public Governorate Governorate { get; set; }
        [Required]
        public Country Country { get; set; } // Country name, not a complex object
        #endregion


        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}