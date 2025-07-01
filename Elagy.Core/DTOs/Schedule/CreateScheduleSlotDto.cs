using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Schedule
{
    public class CreateScheduleSlotDto
    {
        public string DoctorId { get; set; }
        public int DayOfWeekId { get; set; } // Assuming DayOfWeekId corresponds to an enum or a specific ID in your database
        public int HospitalSpecialtyId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int MaxCapacity { get; set; } = 1;
    }
}
