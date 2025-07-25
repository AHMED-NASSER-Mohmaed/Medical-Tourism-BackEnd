﻿using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Auth
{
    public class HotelAssetRegistrationRequestDto : BaseAssetRegistrationRequestDto
    {
        [Range(1,5)]
        public int? StarRating { get; set; }
        public bool? HasPool { get; set; }
        public bool? HasRestaurant { get; set; }

        public HotelAssetRegistrationRequestDto()
        {
            AssetType = Enums.AssetType.Hotel; 
        }
    }
}