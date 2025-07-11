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
        public DateOnly StartingDate { get; set; }
        public DateOnly EndingDate { get; set; }

        //pickup location
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string LocationDescription { get; set; } // Detailed description of the pickup location
        public fuel_policy FuelPolicy { get; set; } // Enum for fuel policy
        //forgien key
        public int CarScheduleId { get; set; }
        // Navigation property
        public CarSchedule CarSchedule { get; set; }
    }
}
