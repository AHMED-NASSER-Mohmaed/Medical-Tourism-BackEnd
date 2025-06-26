using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class Governorate
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Foreign key to Country
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}
