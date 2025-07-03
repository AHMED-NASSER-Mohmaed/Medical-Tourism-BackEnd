using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Driver
{
    public class CarDriverResponseDto
    {
        public int Id { get; set; } 
        public DateOnly AssignmentDate { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public bool IsAssignedCurrent { get; set; }
        public int CarId { get; set; }
        public string CarPlateNumber { get; set; } 
        public string CarMake { get; set; }
        public string CarModelName { get; set; }

        public string DriverId { get; set; }
        public string DriverFirstName { get; set; }
        public string DriverLastName { get; set; }

        // Related CarRentalAsset details (flattened)
        public string CarRentalAssetId { get; set; }
        public string CarRentalAssetName { get; set; }
    }
}
