using System;
using Elagy.Core.Enums;

namespace Elagy.Core.Entities
{
    public abstract class ServiceAsset
    {
        // Id will match the ServiceProvider's Id (which is also the User's Id) for shared primary key relationship.
        public string Id { get; set; }

        public string AssetName { get; set; }
        public string Description { get; set; }
        public string DocsURL { get; set; } // Credential paper documentation for the asset
        public string DocsURLFeildId { get; set; } // Credential paper documentation for the asset
        public string Email { get; set; } // Asset-specific email (e.g., hotel reception email)

        public DateTime AcquisitionDate { get; set; } // Date of asset acquisition/establishment

        //verfication
        public VerificationStatus VerificationStatus { get; set; } // Status of asset verification by SuperAdmin
        //verfication


        public string VerificationNotes { get; set; } // Notes regarding verification status
        public string VerifiedCountryCode { get; set; }

        public AssetType AssetType { get; set; } // Discriminator for TPH

        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public string LocationDescription { get; set; } // Detailed description of the asset's physical location

        public string[] Facilities { get; set; } // Facilities available at the asset (e.g., parking, Wi-Fi, gym)
        public string OpeningHours { get; set; } // Opening hours of the asset (e.g., "Mon-Fri 9am-5pm, Sat 10am-2pm")
        public string[] LanguagesSupported { get; set; } // Languages supported by the asset's staff/services

        // Navigation property to the Principal (ServiceProvider).
        public ServiceProvider ServiceProvider { get; set; }
    }
}

