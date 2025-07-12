using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.CarAppoinment
{
    public class createCarRentalAppoinmentDTO
    {
        public DateOnly StartingDate { get; set; }
        public DateOnly EndingDate { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string LocationDescription { get; set; } 
        public fuel_policy FuelPolicy { get; set; } 
        public int CarId { get; set; } 
    }
}
