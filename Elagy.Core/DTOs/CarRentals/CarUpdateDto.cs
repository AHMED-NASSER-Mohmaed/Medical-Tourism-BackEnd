using Elagy.Core.Enums;
using Imagekit.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.CarRentals
{
    public class CarUpdateDto
    {

        [StringLength(50)] public string? FactoryMake { get; set; }
        [StringLength(50)] public string? ModelName { get; set; }
        [Range(1900, 2100, ErrorMessage = "Invalid model year.")]
        public int? ModelYear { get; set; }
        public CarType? Type { get; set; }
        [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10 passengers.")]
        public int? Capacity { get; set; }
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal? PricePerDay { get; set; }
        public TransmissionType? Transmission { get; set; }
        public FuelType? FuelType { get; set; }
        public bool? IsAvailable { get; set; }
        public CarStatus? Status { get; set; }
        [StringLength(1000)] public string? Description { get; set; }
    }

}
