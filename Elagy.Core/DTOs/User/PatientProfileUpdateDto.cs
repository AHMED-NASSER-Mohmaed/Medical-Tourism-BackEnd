using Elagy.Core.Enums;
using System;

namespace Elagy.Core.DTOs.User
{
    public class PatientProfileUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        


        public Gender Gender { get; set; }

        public string Address { get; set; }
        public string City { get; set; }
        public int GovernorateId { get; set; }
        public int CountryId { get; set; }


        public DateTime? DateOfBirth { get; set; }
        public string BloodGroup { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
    }
}