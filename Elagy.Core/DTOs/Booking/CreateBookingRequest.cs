using Elagy.Core.DTOs.CarAppoinment;
using Elagy.Core.DTOs.RoomAppoinment;
using Elagy.Core.DTOs.SpecialtyAppointment;
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
        public CreateSpecialtyAppointmentDTO SpecialtiyAppointment { get; set; }

        public RoomAppointmentResponseDTO? HotelAppointment { get; set; }


        public CarRentalAppoinmentDTO? CarAppointment { get; set; }

    }
}
