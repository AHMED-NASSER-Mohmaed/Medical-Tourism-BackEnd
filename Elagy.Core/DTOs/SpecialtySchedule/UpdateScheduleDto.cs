using Imagekit.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Schedule
{
    public class UpdateScheduleDto
    {
        public string? DoctorId { get; set; } 
        public int? HospitalSpecialtyId { get; set; } 
        public decimal? Price { get; set; } 

        public int? DayOfWeekId { get; set; } 

        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public TimeSpan? TimeSlotSize { get; set; } 

        [Range(1, int.MaxValue)] public int? MaxCapacity { get; set; }
        public bool? IsActive { get; set; }
    }
}
