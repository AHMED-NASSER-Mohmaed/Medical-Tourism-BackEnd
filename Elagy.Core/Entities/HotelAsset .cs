namespace Elagy.Core.Entities
{
    public class HotelAsset : Asset
    {
        public int? StarRating { get; set; } // E.g., 1 to 5 stars
        public bool? HasPool { get; set; }
        public bool? HasRestaurant { get; set; }

        public ICollection<Room>? Rooms { get; set; }
        public ICollection<HotelAssetImage>? HotelAssetImages { get; set; }
    }
}