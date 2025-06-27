using Elagy.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class HospitalSpecialty
    {
        public int Id { get; set; } // uniqueness of Specialty within a Hospital

        public string HospitalAssetId { get; set; } // Foreign Key to HospitalAsset
        public HospitalAsset HospitalAsset { get; set; }

        public int SpecialtyId { get; set; } // Foreign Key to Specialty
        public Specialty Specialty { get; set; }

        public bool  IsActive { get; set; }

        // Navigation property for Doctors working in this specific Hospital-Specialty combination
        public ICollection<Doctor> Doctors { get; set; }
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}
    
