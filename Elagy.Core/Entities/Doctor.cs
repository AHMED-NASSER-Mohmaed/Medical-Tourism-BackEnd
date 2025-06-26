using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class Doctor : User
    {
        public string MedicalLicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public string Bio { get; set; }
        public string Qualification { get; set; } 
        public int HospitalSpecialtyId { get; set; }
        public HospitalSpecialty HospitalSpecialty { get; set; }

        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
