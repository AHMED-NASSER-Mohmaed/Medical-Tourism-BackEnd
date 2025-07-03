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
    public class DriverUpdateDto
    {
        [StringLength(100)] public string? FirstName { get; set; }
        [StringLength(100)] public string? LastName { get; set; }
        [EmailAddress] public string? Email { get; set; }
        [StringLength(20)] public string? PhoneNumber { get; set; }
        public Gender? Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? UserGovernorateId { get; set; }
        public int? UserCountryId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Status? Status { get; set; } // User's general account status
        [Range(0, 70)] public int? YearsOfExperience { get; set; }
        [Range(0.0f, 5.0f)] public float? Rating { get; set; }
        public DriverStatus? DriverStatus { get; set; } // Driver's specific availability status
    }
}
