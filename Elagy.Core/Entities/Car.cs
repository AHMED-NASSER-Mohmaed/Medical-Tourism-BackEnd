using Elagy.Core.Const;
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
        public int Id { get; set; } 
        [Required] public string FactoryMake { get; set; } 
        [Required] public string ModelName { get; set; }
        [Required] public int ModelYear { get; set; }
        [Required] public CarType Type { get; set; } 
        [Required] public int Capacity { get; set; } 

        [Required] public decimal PricePerDay { get; set; } 
        [Required] public TransmissionType Transmission { get; set; }
        [Required] public FuelType FuelType { get; set; }
        public bool IsAvailable { get; set; } = true; 
        public CarStatus Status { get; set; } 
        public string Description { get; set; } 


        public ICollection<CarImage>? CarImages { get; set; }

        public string CarRentalAssetId { get; set; } // FK to the CarRentalAsset it belongs to
        public CarRentalAsset CarRentalAsset { get; set; } // Navigation property to CarRentalAsset


        public ICollection<CarDriver>? CarDrivers { get; set; }
        public ICollection<CarSchedule> carRentalSchedules { get; set; } = new List<CarSchedule>(); 
    }
}
