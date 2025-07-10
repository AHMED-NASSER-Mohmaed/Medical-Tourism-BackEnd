using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.TOP
{
    public class DoctorBookingCountDto
    {
        public string DoctorId { get; set; }
        public string ImageUrl { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }
        public int YearsOfExperience { get; set; }
        public string HospitalId { get; set; }
        public string HospitalName { get; set; }
        public int BookingCount { get; set; }
    }
}
