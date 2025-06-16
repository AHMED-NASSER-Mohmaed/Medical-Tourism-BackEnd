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

        public string Nationality { get; set; }


        //public string NationalId { get; set; }
        //public string PassportId { get; set; }

        public string? NationalImageKitFileId { get; set; } // ImageKit fileId
        public string? NationalImageUrl { get; set; } // ImageKit URL

        [Required]
        public string Gender { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]

        public string StreetNumber { get; set; }
        [Required]

        public string Governorate { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}