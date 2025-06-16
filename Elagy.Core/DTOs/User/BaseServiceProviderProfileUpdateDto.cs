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
        public string ZipCode { get; set; }
        public string StreetNumber { get; set; }
        public string Governorate { get; set; }
        public DateTime? DateOfBirth { get; set; }


        // Service Asset common properties that can be updated
        public string Description { get; set; }
        public string AssetEmail { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string LocationDescription { get; set; }
        public string[] Facilities { get; set; }
        public string OpeningHours { get; set; }
        public string[] LanguagesSupported { get; set; }

         
        public string VerifiedCountryCode { get; set; }
    }

}