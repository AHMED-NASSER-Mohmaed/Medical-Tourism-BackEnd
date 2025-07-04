using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Booking
{
    public class HospitalAppointmentDTO
    {
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        //forgien key
        public int HotelScheduleId { get; set; }
    }

}
