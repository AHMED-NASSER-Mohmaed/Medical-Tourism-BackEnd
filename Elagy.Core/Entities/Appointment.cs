using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public abstract class Appointment
    {
        ///// <summary>
        ///// Unique identifier for the appointment.
        ///// This will also serve as the primary key in derived tables for TPT.
        ///// </summary>
        //public int Id { get; set; }

        ///// <summary>
        ///// Foreign Key to the Schedule slot this appointment is booked against.
        ///// </summary>
        //public int ScheduleId { get; set; }

        ///// <summary>
        ///// Foreign Key to the Patient who booked this appointment.
        ///// </summary>
        //public string PatientId { get; set; } // Assuming Patient.Id is a string (from IdentityUser)

        ///// <summary>
        ///// The specific date of the appointment.
        ///// </summary>
        //public DateTime AppointmentDate { get; set; }

        ///// <summary>
        ///// The specific start time of the appointment.
        ///// </summary>
        //public TimeOnly AppointmentStartTime { get; set; }

        ///// <summary>
        ///// The specific end time of the appointment.
        ///// </summary>
        //public TimeOnly AppointmentEndTime { get; set; }

        ///// <summary>
        ///// The current status of the appointment (e.g., Booked, Cancelled, Completed).
        ///// </summary>
        //public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;

        ///// <summary>
        ///// The price of this specific appointment.
        ///// </summary>
        //public decimal Price { get; set; }

        // Navigation properties

        /// <summary>
        /// The Schedule slot this appointment is associated with.
        /// </summary>
        //public Schedule Schedule { get; set; }

        /// <summary>
        /// The Patient who booked this appointment.
        /// </summary>
        //public Patient Patient { get; set; }
    }
}
