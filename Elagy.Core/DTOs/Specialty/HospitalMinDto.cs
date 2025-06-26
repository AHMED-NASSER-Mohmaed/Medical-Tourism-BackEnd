using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Specialty
{
    public class HospitalMinDto
    {
        public string Id { get; set; } // Hospital's ServiceAsset Id (which is also ServiceProvider's Id)
        public string Name { get; set; } // AssetName
        //public string ImageURL { get; set; } 
        //public string ImageId { get; set; } 
       
        //public string Governorate { get; set; }
        //public string LocationDescription { get; set; } // General location info
        //public bool HasEmergencyRoom { get; set; } // Specific to HospitalAsset
    }
}
