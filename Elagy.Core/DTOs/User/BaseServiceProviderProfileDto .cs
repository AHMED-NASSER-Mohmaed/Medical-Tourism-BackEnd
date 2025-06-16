using Elagy.Core.Enums;
using System;

namespace Elagy.Core.DTOs.User
{
    public abstract class BaseServiceProviderProfileDto : BaseProfileDto
    {
        public string NationalDocsURL { get; set; }


        // Common ServiceAsset properties flattened directly into this DTO
        public string AssetId { get; set; } // The Id of the associated asset (will be same as User.Id)
        public string AssetName { get; set; }
        public string Description { get; set; }
        public string AssetDocsURL { get; set; }


        public string AssetEmail { get; set; }
        public DateTime AcquisitionDate { get; set; }

        //public VerificationStatus VerificationStatus { get; set; } // Asset's verification status
        public string VerificationNotes { get; set; }
        public string VerifiedCountryCode { get; set; }
        public AssetType AssetType { get; set; } // Discriminator for asset type
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string LocationDescription { get; set; }
        public string[] Facilities { get; set; }
        public string OpeningHours { get; set; }
        public string[] LanguagesSupported { get; set; }
    }
}