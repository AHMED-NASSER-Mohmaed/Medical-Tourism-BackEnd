using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Auth
{
    public class HotelProviderRegistrationRequestDto : BaseServiceProviderRegistrationRequestDto
    {
        public int? StarRating { get; set; }
        public bool? HasPool { get; set; }
        public bool? HasRestaurant { get; set; }

        public HotelProviderRegistrationRequestDto()
        {
            AssetType = Enums.AssetType.Hotel; // Default for this DTO
        }
    }
}