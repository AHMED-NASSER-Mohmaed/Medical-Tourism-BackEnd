using System.ComponentModel.DataAnnotations;
using Elagy.Core.Enums;

namespace Elagy.Core.DTOs.Auth
{
    public abstract class BaseServiceProviderRegistrationRequestDto : BaseRegistrationRequestDto
    {
        public string? ProviderNationalURL { get; set; }
        public string? ProviderNationalFeildId { get; set; }


        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string AssetName { get; set; } = default!;

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Description { get; set; } = default!;

        // NEW: For asset's credential/license file details from ImageKit (sent by client)
        public string? AssetCredentialFileId { get; set; } // ImageKit fileId
        public string? AssetCredentialUrl { get; set; } // ImageKit URL

        public string? AssetEmail { get; set; }
        public string? LocationDescription { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public string[] Facilities { get; set; }
        public string OpeningHours { get; set; }
        public string[] LanguagesSupported { get; set; }

        [Required]
        public AssetType AssetType { get; set; }
    }
}