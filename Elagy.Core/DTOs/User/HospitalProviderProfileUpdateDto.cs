using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.User
{
    public class HospitalProviderProfileUpdateDto : BaseServiceProviderProfileUpdateDto
    {
        public int NumberOfDepartments { get; set; }
        public bool HasEmergencyRoom { get; set; }
        public bool IsTeachingHospital { get; set; }
        public bool EmergencyServices { get; set; }
    }
}
