using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class RoomAppointment:Appointment
    {
        public DateOnly CheckInDate {  get; set; }
        public DateOnly CheckOutDate { get; set; }
        //forgien key
        public int HotelScheduleId { get; set; }
        // Navigation property
        public RoomSchedule RoomSchedule { get; set; }

        public int RoomId { get; set; }

        // Navigation property
        public Room Room { get; set; }
    }
}
