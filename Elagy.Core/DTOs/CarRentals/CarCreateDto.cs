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
    public class CarCreateDto
    {
        [Required(ErrorMessage = "Make is required.")]
        [StringLength(50)] public string FactoryMake { get; set; } = default!;

        [Required(ErrorMessage = "Model name is required.")]
        [StringLength(50)] public string ModelName { get; set; } = default!;

        [Required(ErrorMessage = "Model year is required.")]
        [Range(1900, 2100, ErrorMessage = "Invalid model year.")]
        public int ModelYear { get; set; }

        [Required(ErrorMessage = "Car type is required.")]
        public CarType Type { get; set; }

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 10, ErrorMessage = "Capacity must be between 1 and 10 passengers.")]
        public int Capacity { get; set; }

        // DEFINITIVE: PlateNumber and Color are NOT included here.

        [Required(ErrorMessage = "Price per day is required.")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal PricePerDay { get; set; }

        [Required(ErrorMessage = "Transmission type is required.")]
        public TransmissionType Transmission { get; set; }

        [Required(ErrorMessage = "Fuel type is required.")]
        public FuelType FuelType { get; set; }

        public bool IsAvailable { get; set; } = true;
        public CarStatus Status { get; set; } = CarStatus.Available;
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = default!;


        // CarRentalAssetId is typically inferred from the authenticated admin's token in service layer.
    }
}
