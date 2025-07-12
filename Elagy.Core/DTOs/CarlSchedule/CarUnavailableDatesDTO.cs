using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.CarlSchedule
{
    public class Periode
    {
        public DateOnly StartingDate { get; set; }
        public DateOnly EndingDate { get; set; }

    }
    public class CarUnavailableDatesDTO
    {
        public int CarId { get; set; }
        public string CarModel { get; set; }
        public string CarRentalId { get; set; }
        public string CarRentalName { get; set; }

        public List<Periode> UnavailableDates { get; set; }

    }


}
