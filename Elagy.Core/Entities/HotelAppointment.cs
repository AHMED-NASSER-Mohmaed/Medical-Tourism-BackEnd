using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class HotelAppointment:Appointment
    {
        DateOnly CheckInDate {  get; set; }
        DateOnly CheckOutDate { get; set; }

        //other attributes for hotel appointment : i done know about the hotel bussiness

        //forgien key    
        int HotelScheduleId { get; set; }
        // Navigation property
        HotelSchedule HotelSchedule { get; set; }

    }
}
