using Elagy.Core.DTOs.User;

public class HotelProviderProfileDto : BaseServiceProviderProfileDto
{
    public int? StarRating { get; set; }
    public bool? HasPool { get; set; }
    public bool? HasRestaurant { get; set; }
}