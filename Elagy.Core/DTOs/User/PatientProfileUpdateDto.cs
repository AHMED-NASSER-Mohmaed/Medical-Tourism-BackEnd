using System;

namespace Elagy.Core.DTOs.User
{
    public class PatientProfileUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Nationality { get; set; }
        public string NationalId { get; set; }
        public string PassportId { get; set; }
        public string ImageId { get; set; }
        public string ImageURL { get; set; }
        public string Gender { get; set; }
        public string ZipCode { get; set; }
        public string StreetNumber { get; set; }
        public string Governorate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string BloodGroup { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
    }
}