using System;
using System.ComponentModel.DataAnnotations;
using Elagy.Core.Enums;

namespace Elagy.Core.DTOs.User
{
    public abstract class BaseServiceProviderProfileUpdateDto
    {
        // User (Service Provider) properties that can be updated
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        #region
        public string Address { get; set; } // Full address as a single string
        public string City { get; set; } // City name, not a complex object
        public Governorate Governorate { get; set; }
        public Country Country { get; set; } // Country name, not a complex object
        #endregion
        public DateTime? DateOfBirth { get; set; }





        // Service Asset common properties that can be updated

        #region //related to location
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string LocationDescription { get; set; } // Detailed description of the asset's physical location
        #endregion
        public string[] Facilities { get; set; } // Facilities available at the asset (e.g., parking, Wi-Fi, gym)
        public Language[] LanguagesSupported { get; set; } // Languages supported by the asset's staff/services

        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }
        
    }

}