using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Schedule
{
    public class ScheduleResponseDto
    {
        public int Id { get; set; }
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
        public string Hospital { get; set; }
        public DayOfWeekDto DayOfWeek { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int MaxCapacity { get; set; }
        public int AvailableSlots => MaxCapacity - BookedSlots;
        public int BookedSlots { get; set; }
        public bool? IsActive { get; set; } 

    }
}
