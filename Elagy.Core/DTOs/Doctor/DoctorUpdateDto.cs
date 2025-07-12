using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Doctor
{
    public class DoctorUpdateDto
    {


        [StringLength(100)]
        public string? FirstName { get; set; }


        [StringLength(100)]
        public string? LastName { get; set; }


        public string? Email { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public Gender? Gender { get; set; }

        public int GovernorateId { get; set; }
        public int CountryId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? Address { get; set; } // Full address as a single string
        public string? City { get; set; }


        [StringLength(50)]
        public string? MedicalLicenseNumber { get; set; }

        public int? YearsOfExperience { get; set; }

        [StringLength(1000)]
        public string? Bio { get; set; }

        [StringLength(100)]
        public string? Qualification { get; set; }

        public int HospitalSpecialtyId { get; set; } // id => dose it map to hospital id + clinic id 


        public Status Status { get; set; } // Admin can change status (Active, Inactive, etc.)
    }
}
