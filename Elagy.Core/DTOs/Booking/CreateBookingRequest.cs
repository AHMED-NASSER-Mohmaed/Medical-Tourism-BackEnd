using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Booking
{
    public class CreateBookingRequest
    {
        [Required]
        public HospitalAppointmentDTO HospitalAppointment { get; set; }

        public CarRentalAppoinmentDTO? CarRentalAppointment { get; set; }

        public HotelAppointmentDTO? HotelAppointment { get; set; }



    }
}
