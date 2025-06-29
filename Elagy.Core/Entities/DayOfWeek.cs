using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class DayOfWeek
    {
        public int Id { get; set; }
        public string Name { get; set; } // "Monday", "Tuesday", etc.
        public string ShortCode { get; set; } // "MON", "TUE"

    
        public ICollection<Schedule> Schedules { get; set; }
    }
}
