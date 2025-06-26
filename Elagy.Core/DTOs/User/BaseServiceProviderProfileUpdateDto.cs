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
        public string Phone{ get; set; }
        public string Gender { get; set; }

        #region
        public string Address { get; set; }  
        public string City { get; set; } 
        public int GovernorateId { get; set; }
        public int CountryId { get; set; } 
        #endregion
        public DateTime? DateOfBirth { get; set; }



        public string AssetName {  get; set; }
        public string AsetDescription { get; set; }

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