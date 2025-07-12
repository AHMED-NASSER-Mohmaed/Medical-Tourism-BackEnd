using Elagy.Core.DTOs.Driver;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.CarRentals
{
  
       public class CarResponseDto
        {
            public int Id { get; set; }
            public string FactoryMake { get; set; }
            public string ModelName { get; set; }
            public int ModelYear { get; set; }
            public CarType Type { get; set; }
            public int Capacity { get; set; }
            public decimal PricePerDay { get; set; }
            public TransmissionType Transmission { get; set; }
            public FuelType FuelType { get; set; }
            public bool IsAvailable { get; set; }
            public CarStatus Status { get; set; }
            public string Description { get; set; }
            public ICollection<CarImageDto>? CarImages { get; set; }

            // Related CarRentalAsset details
            public string CarRentalAssetId { get; set; }
            public string CarRentalAssetName { get; set; }
        
    }
}
