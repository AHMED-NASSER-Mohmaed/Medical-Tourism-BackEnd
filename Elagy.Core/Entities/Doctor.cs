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
        public string Qualification { get; set; } // e.g., MD, MBBS, PhD

        // Foreign Key to the HospitalSpecialty combination the doctor works in
        public int HospitalSpecialtyId { get; set; }
        public HospitalSpecialty HospitalSpecialty { get; set; }

        //// Foreign Key to the Specialty the doctor primarily works in (redundant but can be useful for direct lookup)
        //public int SpecialtyId { get; set; }
        //public Specialty Specialty { get; set; }
    }
}
