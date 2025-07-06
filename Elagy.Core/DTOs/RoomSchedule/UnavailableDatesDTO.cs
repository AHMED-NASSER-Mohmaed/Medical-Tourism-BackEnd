using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.RoomSchedule
{
    public class UnavailableDatesDTO
    {
        public int RoomId { get; set; }
        public IEnumerable<DateOnly> UnavailableDates { get; set; }

    }
}
