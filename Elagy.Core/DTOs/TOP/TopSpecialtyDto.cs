using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.TOP
{
    public class TopSpecialtyDto
    {
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }
        public int BookingCount { get; set; }
        public int DoctorCount { get; set; }
        public string? IconUrl { get; set; } 
    }
}
