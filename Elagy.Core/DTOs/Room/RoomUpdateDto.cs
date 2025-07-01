using Elagy.Core.Enums;
using Imagekit.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Room
{
    public class RoomUpdateDto
    {
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal? Price { get; set; } 

        [Range(1, 20, ErrorMessage = "Max occupancy must be between 1 and 20.")]
        public int? MaxOccupancy { get; set; } 

        public bool? IsAvailable { get; set; } 

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        public string[]? Amenities { get; set; } 

        [StringLength(50, ErrorMessage = "Room number cannot exceed 50 characters.")]
        public string? RoomNumber { get; set; } 

        public int? FloorNumber { get; set; } 

        public bool? HasBalcony { get; set; } 

        public ViewType? ViewType { get; set; } 

        public RoomStatus? Status { get; set; } 

        public bool? IncludesBreakfast { get; set; }

        public RoomCategory? RoomType { get; set; } // Nullable Enum, for changing room category
    }
}
