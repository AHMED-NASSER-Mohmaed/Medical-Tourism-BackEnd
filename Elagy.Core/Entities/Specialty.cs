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

        public bool IsActive { get; set; } = true;  

        // Navigation property for the many-to-many relationship with HospitalAsset
        public ICollection<HospitalSpecialty>? HospitalSpecialties { get; set; }

    }
}
