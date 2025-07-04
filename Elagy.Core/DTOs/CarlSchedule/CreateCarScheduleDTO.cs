using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.CarRentalSchedule
{
    public class CreateCarScheduleDTO
    {

        public DateOnly StartingDate { get; set; } // Start date of the rental schedule
        public DateOnly EndingDate { get; set; } // End date of the rental schedule

        public int CarId { get; set; } // Foreign key for the car associated with this schedule

    }
}
