using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Temps
{
    public class ScheduleDateKey
    {
        public DateTime Date { get; set; }
        public int SpecialtyScheduleId { get; set; }

        public override bool Equals(object obj) =>
            obj is ScheduleDateKey other &&
            Date.Date == other.Date.Date &&
            SpecialtyScheduleId == other.SpecialtyScheduleId;

        public override int GetHashCode() =>
            HashCode.Combine(Date.Date, SpecialtyScheduleId);
    }

}
