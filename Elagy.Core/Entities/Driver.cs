using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class Driver : User 
    {
        public string DriveLicenseLicenseNumberURL { get; set; }
        public string DriveLicenseLicenseNumberId { get; set; }
        [Required] public int YearsOfExperience { get; set; }
        public string CarRentalAssetId { get; set; }
        public DriverStatus DriverStatus { get; set; } 
        public CarRentalAsset CarRentalAsset { get; set; } 
        public ICollection<CarDriver>? CarDrivers { get; set; }
    }
}
