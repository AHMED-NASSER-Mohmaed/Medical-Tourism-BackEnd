using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class CarSchedule
    {
        public int Id { get; set; } // Unique identifier for the schedule
        public decimal TotalPrice { get; set; } // Price for the rental schedule, can be calculated based on the car's price per day and the duration of the schedule
        public DateOnly StartingDate { get; set; } // Start date of the rental schedule
        public DateOnly EndingDate { get; set; } // End date of the rental schedule
        public int CarId { get; set; } // Foreign key for the car associated with this schedule
        public Car Car { get; set; } // Navigation property to the Car entity
        public ScheduleStatus Status { get; set; } = ScheduleStatus.Pending; // Status of the schedule (e.g., Active, Completed, Cancelled)
        public List<CarRentalAppointment> CarRentalAppointments { get; set; } = new List<CarRentalAppointment>();
    }
}
