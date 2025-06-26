using System.ComponentModel.DataAnnotations;
using Elagy.Core.Enums;

namespace Elagy.Core.DTOs.Auth
{
    public abstract class BaseAssetRegistrationRequestDto : BaseRegistrationRequestDto
    {
        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string AssetName { get; set; } = default!;

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string AssetDescription { get; set; } = default!;

        [Required]
        public string AssetEmail { get; set; }
        [Required]
        public string LocationDescription { get; set; }
        [Required]
        public float Latitude { get; set; }
        [Required]
        public float Longitude { get; set; }

        public string[] Facilities { get; set; }

        public string VerificationNotes { get; set; } 

        [Required]
        TimeOnly OpeningTime { get; set; }
        [Required]
        TimeOnly ClosingTime { get; set; }


        [Required]
        public Language[] LanguagesSupported { get; set; }

        [Required]
        public AssetType AssetType { get; set; }
    }
}