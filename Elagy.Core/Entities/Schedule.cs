using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class Schedule
    {
  
        public int Id { get; set; }

        public string DoctorId { get; set; }

        public int HospitalSpecialtyId { get; set; }

        public DateTime Date { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }

        public int MaxCapacity { get; set; }

        public int BookedSlots { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public Doctor Doctor { get; set; }

        public HospitalSpecialty HospitalSpecialty { get; set; }

        //public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
