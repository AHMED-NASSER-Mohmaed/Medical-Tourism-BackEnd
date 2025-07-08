using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.RoomSchedule
{
    public class RoomScheduleResponseDTO
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public ScheduleStatus RoomscheduleStatus { get; set; }
        public List<DateOnly> UnavailableDates { get; set; }=new List<DateOnly>();
        public int RoomId { get; set; }
    }
}
