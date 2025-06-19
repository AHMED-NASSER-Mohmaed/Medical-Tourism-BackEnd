using System;
using Elagy.Core.Enums;

namespace Elagy.Core.Entities
{
    public abstract class Asset
    {
        // Id will match the ServiceProvider's Id (which is also the User's Id) for shared primary key relationship.
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string CredentialDocURL { get; set; } // Credential paper documentation for the asset
        public string CredentialDocId { get; set; } // Credential paper documentation for the asset
        public string Email { get; set; } // Asset-specific email (e.g., hotel reception email)
        public DateTime AcquisitionDate { get; set; } = DateTime.Now; // Date of asset acquisition/establishment
        public string VerificationNotes { get; set; } // Notes regarding verification status

        #region //related to location
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string LocationDescription { get; set; } // Detailed description of the asset's physical location
        #endregion
        public string[] Facilities { get; set; } // Facilities available at the asset (e.g., parking, Wi-Fi, gym)
        public Language[] LanguagesSupported { get; set; } // Languages supported by the asset's staff/services
        
        public TimeOnly OpeningTime { get; set; }
        public TimeOnly ClosingTime { get; set; }

        public AssetType AssetType { get; set; } // Discriminator for TPH

        // Navigation property to the Principal (ServiceProvider).
        public ServiceProvider ServiceProvider { get; set; }

    }
}

