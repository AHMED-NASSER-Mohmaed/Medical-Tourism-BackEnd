using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Driver
{
    public class DriverResponseDto
    {
        // User Base Properties
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string? ImageURL { get; set; }
        public Gender Gender { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? governerateId { get; set; } // Direct object for display
        public int? countryId { get; set; } // Direct object for display
        public string GovernarteName { get; set; }
        public string CountryName { get; set; }

        public DateTime? DateOfBirth { get; set; }
        public bool EmailConfirmed { get; set; }
        public Status Status { get; set; } // User's general account status
        public UserType UserType { get; set; }


        public string DriveLicenseLicenseNumberURL { get; set; }
        public int YearsOfExperience { get; set; }
        public float Rating { get; set; }
        public DriverStatus DriverStatus { get; set; } // Driver's current availability status

        // CarRentalAsset affiliation details
        public string CarRentalAssetId { get; set; }
        public string CarRentalAssetName { get; set; } // Name of the associated car rental

        // Car assignments (e.g., current active assignment for display)
        public ICollection<CarDriverResponseDto>? CarDrivers { get; set; } // Assuming CarDriverResponseDto is defined below
    }
}
