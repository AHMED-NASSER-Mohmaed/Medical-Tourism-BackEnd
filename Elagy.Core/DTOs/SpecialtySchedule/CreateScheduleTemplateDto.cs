using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Schedule
{
    public class CreateScheduleTemplateDto
    { //this for future to make a job create the same schedule every specific time
        public string DoctorId { get; set; }
        public int HospitalSpecialtyId { get; set; }
        public int DayOfWeekId { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int MaxCapacity { get; set; } = 1;
        public bool IsActive { get; set; } = true;
    }
}
