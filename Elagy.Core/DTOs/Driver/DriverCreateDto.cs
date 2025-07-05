using Elagy.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Driver
{
    public class DriverCreateDto
    {
        // User Base Properties
        [Required][EmailAddress] public string Email { get; set; } = default!;
        [Required][MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")] public string Password { get; set; } = default!;
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")] public string ConfirmPassword { get; set; } = default!;
        [Required][StringLength(100, MinimumLength = 2)] public string FirstName { get; set; } = default!;
        [Required][StringLength(100, MinimumLength = 2)] public string LastName { get; set; } = default!;
        [Required] public string Phone { get; set; } = default!;
        [Required] public Gender Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        [Required] public int UserGovernorateId { get; set; }
        [Required] public int UserCountryId { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Years of experience are required.")]
        [Range(0, 70, ErrorMessage = "Years of experience must be between 0 and 70.")]
        public int YearsOfExperience { get; set; }
        public float Rating { get; set; } = 0.0f;
        public DriverStatus DriverStatus { get; set; } = DriverStatus.Available;

    }
}
