using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public  class Car
    {
        public int Id { get; set; } // Primary Key for Car
        [Required] public string FactoryMake { get; set; } // e.g., "Toyota"
        [Required] public string ModelName { get; set; } // e.g., "Camry"
        [Required] public int ModelYear { get; set; } // e.g., 2022
        [Required] public CarType Type { get; set; } // e.g., Sedan, SUV
        [Required] public int Capacity { get; set; } // Max passengers

        public decimal Price { get; set; } // Base price of the car, can be used for purchase or rental

        // Rental Specifics
        [Required] public decimal PricePerDay { get; set; } // Daily rental price
        [Required] public TransmissionType Transmission { get; set; }
        [Required] public FuelType FuelType { get; set; }
        public bool IsAvailable { get; set; } = true; // General operational availability for rental
        public CarStatus Status { get; set; } // Internal status (Available, OnRide, UnderMaintenance)
        public string Description { get; set; } // Detailed description of the car


        public ICollection<CarImage>? CarImages { get; set; }

        public string CarRentalAssetId { get; set; } // FK to the CarRentalAsset it belongs to
        public CarRentalAsset CarRentalAsset { get; set; } // Navigation property to CarRentalAsset


        public ICollection<CarDriver>? CarDrivers { get; set; }
        public ICollection<CarSchedule> carRentalSchedules { get; set; } = new List<CarSchedule>(); // Appointments for this car
    }
}
