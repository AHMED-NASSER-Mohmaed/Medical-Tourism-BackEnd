﻿using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Doctor
{
    public class DoctorProfileDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; } // Inherited from IdentityUser [from username]
        public string? ImageURL { get; set; }
        public Gender Gender { get; set; }

        #region doctor address
        public string Address { get; set; }
        public string City { get; set; }
        public int GovernorateId { get; set; }
        public string GovernorateName { get; set; }
        // MODIFIED: Replace Country object with ID and Name
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        #endregion

        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Status Status { get; set; }

        public string MedicalLicenseNumber { get; set; }
        public int YearsOfExperience { get; set; }
        public string Bio { get; set; }
        public string Qualification { get; set; }




        // Associated Specialty information
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }


        public string HospitalId { get; set; } // The hospital the doctor is affiliated with
        public string HospitalName { get; set; } // Name of the hospital
    }
}
