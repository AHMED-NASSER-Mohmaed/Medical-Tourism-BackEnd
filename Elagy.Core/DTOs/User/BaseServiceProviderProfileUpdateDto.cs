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
        public string Nationality { get; set; }
        public string NationalId { get; set; }
        public string PassportId { get; set; }
        public string ImageId { get; set; }
        public string ImageURL { get; set; }
        public string Gender { get; set; }
        public string ZipCode { get; set; }
        public string StreetNumber { get; set; }
        public string Governorate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ProviderDocsURL { get; set; }

        // Service Asset common properties that can be updated
        public string AssetName { get; set; }
        public string Description { get; set; }
        public string AssetDocsURL { get; set; }
        [EmailAddress] // Asset email can be changed
        public string AssetEmail { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string LocationDescription { get; set; }
        public string[] Facilities { get; set; }
        public string OpeningHours { get; set; }
        public string[] LanguagesSupported { get; set; }

        // Admin-only updates for asset verification, not exposed to regular providers
        public VerificationStatus? VerificationStatus { get; set; } // Nullable, as admin might not update this every time
        public string VerificationNotes { get; set; }
        public string VerifiedCountryCode { get; set; }
    }

}