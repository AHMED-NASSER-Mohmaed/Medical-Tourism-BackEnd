using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Doctor
{
    public class DoctorTableDto
    {
        public string Id { get; set; } 
        public string Name { get; set; } 
        public string? Email { get; set; } 
        public string Specialist { get; set; } 
    }
}
