using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class CarDriver
    {
        public int Id { get; set; }
        // Composite Primary Key for the join table
        public string CarRentalAssetId { get; set; } 
        public int CarId { get; set; }
        public string DriverId { get; set; } // DriverId is string (from User)

        public DateOnly AssignmentDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public DateOnly? ReleaseDate { get; set; } 
        public bool IsAssignedCurrent { get; set; } = true;

        public CarRentalAsset CarRentalAsset { get; set; }
        public Car Car { get; set; }
        public Driver Driver { get; set; }
    }

}
