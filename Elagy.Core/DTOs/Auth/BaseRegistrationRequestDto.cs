using System;
using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Auth
{
    public abstract class BaseRegistrationRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; } // From IdentityUser, add here for registration

        // Optional common user attributes
        public string? Nationality { get; set; }
        public string? NationalId { get; set; }
        public string? PassportId { get; set; }
        public string? ImageId { get; set; }
        public string? ImageURL { get; set; }
        public string Gender { get; set; }
        public string ZipCode { get; set; }
        public string StreetNumber { get; set; }
        public string Governorate { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}