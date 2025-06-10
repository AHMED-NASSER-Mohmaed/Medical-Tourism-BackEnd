using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.User
{
    public class CarRentalProviderProfileUpdateDto : BaseServiceProviderProfileUpdateDto
    {
        public string OperationalAreas { get; set; }
        public string[] VehicleType { get; set; }
        public string[] Transmission { get; set; }
        public string[] FuelType { get; set; }
        public string[] RentalPolicies { get; set; }
        public string[] AdditionalServices { get; set; }
        public string[] CarFeatures { get; set; }
    }

}
