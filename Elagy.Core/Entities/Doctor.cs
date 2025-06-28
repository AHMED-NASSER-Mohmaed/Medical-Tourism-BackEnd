using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class Doctor : User
    {
        //doctor certificate
        public string MedicalLicenseNumberURL { get; set; }
        public string MedicalLicenseNumberId { get; set; }

        public int YearsOfExperience { get; set; }
        public string Bio { get; set; }
        public string Qualification { get; set; } 

        //hospital forgin key
        public int HospitalSpecialtyId { get; set; }

        //refernce key to specialty at specific hospital 
        public HospitalSpecialty HospitalSpecialty { get; set; }

        public ICollection<Schedule>?Schedules { get; set; } 
    }
}
