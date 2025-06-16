using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class Specialty
    {
        public int Id { get; set; } // Primary Key for Specialty
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool IsDeleted { get; set; } = false; // Default to not deleted
        public DateTime? DeletedAt { get; set; }

        // Navigation property for the many-to-many relationship with HospitalAsset
        public ICollection<HospitalSpecialty>? HospitalSpecialties { get; set; }

        //// Navigation property for Doctors working in this specialty
        //public ICollection<Doctor> Doctors { get; set;}
    }
}
