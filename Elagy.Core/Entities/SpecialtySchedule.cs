using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class SpecialtySchedule
    {
        public int Id { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public TimeSpan TimeSlotSize {  get; set; }
        public int MaxCapacity { get; set; }

        //public int BookedSlots { get; set; } = 0; // Driven

        //added by nasser ya mahdy
        //public int CancelledSlots { get; set; } = 0; // Driven
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public int DayOfWeekId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public int HospitalSpecialtyId { get; set; }
        public HospitalSpecialty HospitalSpecialty { get; set; }

        public ICollection<SpecialtyAppointment> Appointments { get; set; } = new List<SpecialtyAppointment>();
    }
}
