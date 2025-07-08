using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Schedule
{
    public class CreateScheduleSlotDto
    {
        [Required] public string DoctorId { get; set; }
        [Required] public int HospitalSpecialtyId { get; set; }

        [Required]
        [Range(1, 7, ErrorMessage = "Day of Week ID must be between 1 and 7.")]
        public int DayOfWeekId { get; set; }

        [Required] public TimeOnly StartTime { get; set; }
        [Required] public TimeOnly EndTime { get; set; }
        [Required] public TimeSpan TimeSlotSize { get; set; }

        [Required][Range(1, int.MaxValue)] public int MaxCapacity { get; set; } = 1;
        [Required][Range(0.01, (double)decimal.MaxValue)] public decimal Price { get; set; }

    }
}
