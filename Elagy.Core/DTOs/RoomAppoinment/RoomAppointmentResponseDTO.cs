using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.RoomAppoinment
{
    public class RoomAppointmentResponseDTO
    {
        public int Id { get; set; }

        //if price changed at the clinic
        public decimal price { get; set; }

        //------ Hospital ---- Hotle ----- CarRental
        public AppointmentType Type { get; set; }

        public AppointmentStatus Status { get; set; }
        public Guid PackageId { get; set; }
            
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        //forgien key
        public int HotelScheduleId { get; set; }
        // Navigation property
        public int RoomId { get; set; }

    }
}
