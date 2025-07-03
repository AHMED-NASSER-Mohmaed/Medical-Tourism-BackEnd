using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elagy.Core.Enums;

namespace Elagy.Core.Entities
{
    public class CarRentalAppointment:Appointment
    {
        DateTime StartingDateTime { get; set; }
        DateTime EndingDateTime { get; set; }

        //pickup location
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string LocationDescription { get; set; } // Detailed description of the pickup location


        //other attributes for hotel appointment : i done know about the CarRental bussiness


        public fuel_policy FuelPolicy { get; set; } // Enum for fuel policy




        //forgien key
        int CarRentalScheduleId { get; set; }
        // Navigation property
        public CarRentalSchedule CarRentalSchedule { get; set; }
    }
}
