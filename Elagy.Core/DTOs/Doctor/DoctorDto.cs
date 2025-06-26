using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Doctor
{
    public class DoctorDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } // Inherited from IdentityUser
        public string? ImageURL { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public Governorate Governorate { get; set; }
        public Country Country { get; set; }
        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public UserStatus Status { get; set; }
        public string MedicalLicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public string Bio { get; set; }
        public string Qualification { get; set; }

        // Associated Specialty information
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }
        public string HospitalAssetId { get; set; } // The hospital the doctor is affiliated with
        public string HospitalName { get; set; } // Name of the hospital
    }
}
