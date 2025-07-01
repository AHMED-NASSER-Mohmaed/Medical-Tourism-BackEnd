using Elagy.Core.Enums;
using System;

namespace Elagy.Core.DTOs.User
{
    public abstract class BaseServiceProviderProfileDto : BaseProfileDto
    {
        public string NationalDocsURL { get; set; }
        public DateTime AcquisitionDate { get; set; }


        // Common ServiceAsset properties flattened directly into this DTO
        public string AssetId { get; set; } // The Id of the associated asset (will be same as User.Id)
        public string AssetName { get; set; }
        public string Description { get; set; }
        public string CredentialDocURL { get; set; }


        public string AssetEmail { get; set; }

        //public VerificationStatus VerificationStatus { get; set; } // Asset's verification status
        public string VerificationNotes { get; set; }


        public int AssetGovernateId{ get; set; }
        public string AssetGovernateName { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string LocationDescription { get; set; }
        public string[] Facilities { get; set; }
        public  TimeOnly OpeningTime { get; set; }
        public  TimeOnly ClosingTime { get; set; }
        public string[] LanguagesSupported { get; set; }
        public AssetType AssetType { get; set; } // Discriminator for asset type
    }
}