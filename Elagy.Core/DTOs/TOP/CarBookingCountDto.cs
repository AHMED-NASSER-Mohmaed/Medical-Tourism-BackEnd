using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.TOP
{
    public class CarBookingCountDto
    {
        public int CarId { get; set; }
        public string ImageUrl { get; set; }  
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string CarRentalName {  get; set; }
        public string CarRentalId { get; set; }
        public int RentalCount { get; set; }  
 
    }
}
