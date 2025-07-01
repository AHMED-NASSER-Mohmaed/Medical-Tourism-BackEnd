using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Room
{
    public class RoomResponseDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int MaxOccupancy { get; set; }
        public bool IsAvailable { get; set; }
        public string Description { get; set; }
        public string[]? Amenities { get; set; }
        public string RoomNumber { get; set; }
        public int FloorNumber { get; set; }
        public bool HasBalcony { get; set; }
        public ViewType ViewType { get; set; }
        public RoomStatus Status { get; set; } 
        public bool IncludesBreakfast { get; set; }
        public ICollection<RoomImageDto>? RoomImages { get; set; }
        public RoomCategory RoomType { get; set; } 
        public string HotelAssetId { get; set; }
        public string HotelAssetName { get; set; }
        public int? HotelStarRating { get; set; } 
    }
}
