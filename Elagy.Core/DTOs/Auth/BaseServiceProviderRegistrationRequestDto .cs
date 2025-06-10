using System.ComponentModel.DataAnnotations;
using Elagy.Core.Enums; // For AssetType

namespace Elagy.Core.DTOs.Auth
{
    public abstract class BaseServiceProviderRegistrationRequestDto : BaseRegistrationRequestDto
    {
        public string ProviderDocsURL { get; set; } // From ServiceProvider entity

        // Common ServiceAsset properties included directly
        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string AssetName { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Description { get; set; }

        public string AssetDocsURL { get; set; } // Specific docs for the asset
        [EmailAddress]
        public string AssetEmail { get; set; } // Asset-specific email (e.g., reception email)

        [Required]
        public float Latitude { get; set; }
        [Required]
        public float Longitude { get; set; }
        public string LocationDescription { get; set; }

        public string[] Facilities { get; set; }
        public string OpeningHours { get; set; }
        public string[] LanguagesSupported { get; set; }

        [Required]
        public AssetType AssetType { get; set; } // Crucial for defining which concrete asset to create
    }
}