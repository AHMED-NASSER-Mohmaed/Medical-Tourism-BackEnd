using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.RoomAppoinment
{
    public class CreateRoomAppointmentDTO
    {
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        //forgien key
        public int RoomId { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
