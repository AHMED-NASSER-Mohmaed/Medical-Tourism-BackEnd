using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.RoomSchedule
{
    public class CreateRoomScheduleDTO
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int RoomId { get; set; }
    }
}
