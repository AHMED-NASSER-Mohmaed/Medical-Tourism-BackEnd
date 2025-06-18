using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.User
{
    public class CarRentalProviderProfileUpdateDto : BaseServiceProviderProfileUpdateDto
    {
         public Governorate[] OperationalAreas { get; set; }  

        public FuelType[] FuelTypes { get; set; }

        public string[] Models { get; set; }  

        public TransmissionType Transmission { get; set; }    
        public string[] RentalPolicies { get; set; } 
    }

}
