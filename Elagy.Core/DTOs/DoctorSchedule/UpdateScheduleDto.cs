using Imagekit.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.DoctorSchedule
{
    public class UpdateScheduleDto
    {
        [Required(ErrorMessage = "Schedule ID is required for update.")]
        [Range(1, int.MaxValue, ErrorMessage = "Schedule ID must be a positive integer.")]
        public int Id { get; set; }

        // DoctorId and HospitalSpecialtyId are typically not changed directly via update,
        // but can be included if your business logic allows re-assigning a schedule slot.
        // For simplicity, we'll allow it as optional, but service will validate.
        public string? DoctorId { get; set; } // Can change the doctor for this slot
        public int? HospitalSpecialtyId { get; set; } // Can change the HS for this slot

        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime? Date { get; set; }

        [DataType(DataType.Time, ErrorMessage = "Invalid time format.")]
        public TimeOnly? StartTime { get; set; }

        [DataType(DataType.Time, ErrorMessage = "Invalid time format.")]
        public TimeOnly? EndTime { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Max capacity must be at least 1.")]
        public int? MaxCapacity { get; set; }

        public bool? IsActive { get; set; } // Allows activating/deactivating a schedule
    }
}
