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
    public class RoomCreateDto
    {
        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Max occupancy is required.")]
        [Range(1, 20, ErrorMessage = "Max occupancy must be between 1 and 20.")]
        public int MaxOccupancy { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = default!;

        public string[]? Amenities { get; set; }

        [Required(ErrorMessage = "Room number is required.")]
        [StringLength(50, ErrorMessage = "Room number cannot exceed 50 characters.")]
        public string RoomNumber { get; set; } = default!;

        [Required(ErrorMessage = "Floor number is required.")]
        [Range(0, 100, ErrorMessage = "Floor number must be between 0 (Ground) and 100.")]
        public int FloorNumber { get; set; }

        [Required(ErrorMessage = "Has balcony status is required.")]
        public bool HasBalcony { get; set; }

        [Required(ErrorMessage = "View type is required.")]
        public ViewType ViewType { get; set; }

        [Required(ErrorMessage = "Initial room status is required.")]
        public RoomStatus Status { get; set; }

        [Required(ErrorMessage = "Includes breakfast status is required.")]
        public bool IncludesBreakfast { get; set; }

        [Required(ErrorMessage = "Room type (category) is required.")]
        public RoomCategory RoomType { get; set; }

    }
}
