using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Locations
{

    //FOR GETTING   
 

    public class Countries_GovernatesDTO
    {
        public Dictionary<int, Country_GovernatesDTO> Data { get; set; } = new();
 
    }

    public class Country_GovernatesDTO
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }

        public Dictionary<int, GovernateDTO> Governates { get; set; }

    }

    public class GovernateDTO
    {
       public int GovernateId { get; set; }
       public string GovernateName { get; set; }

    }

}
