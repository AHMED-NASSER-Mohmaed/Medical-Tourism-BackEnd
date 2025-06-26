using Imagekit.Constant;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.DoctorSchedule
{
    public class CreateScheduleDto
    {
        [Required(ErrorMessage = "Doctor ID is required.")]
        public string DoctorId { get; set; }

        [Required(ErrorMessage = "Hospital Specialty ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Hospital Specialty ID must be a positive integer.")]
        public int HospitalSpecialtyId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format.")]
        public DateTime Date { get; set; } // Will store only the date part in backend logic

        [Required(ErrorMessage = "Start time is required.")]
        [DataType(DataType.Time, ErrorMessage = "Invalid time format.")]
        public TimeOnly StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [DataType(DataType.Time, ErrorMessage = "Invalid time format.")]
        public TimeOnly EndTime { get; set; }

        [Required(ErrorMessage = "Max capacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Max capacity must be at least 1.")]
        public int MaxCapacity { get; set; }


        public bool IsActive { get; set; } = true;
    }
}
