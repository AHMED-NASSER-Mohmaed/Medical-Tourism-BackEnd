using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.DoctorSchedule
{
    /// <summary>
    /// DTO for returning schedule details to the frontend.
    /// This DTO combines information from Schedule, Doctor, HospitalAsset, and Specialty.
    /// </summary>
    public class ScheduleDto
    {
        public int Id { get; set; }
        public string DoctorId { get; set; }
        public string DoctorFirstName { get; set; }
        public string DoctorLastName { get; set; }
        public int HospitalSpecialtyId { get; set; }
        public string HospitalName { get; set; }
        public string SpecialtyName { get; set; }
        public DateTime Date { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int MaxCapacity { get; set; }
        public int BookedSlots { get; set; }
        public bool IsActive { get; set; }
    }
}
