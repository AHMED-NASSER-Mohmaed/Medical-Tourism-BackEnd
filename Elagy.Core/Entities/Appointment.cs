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
         
        public int Id { get; set; }

        public DateOnly Date { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;
        public string PatientId { get; set; } // Assuming Patient.Id is a string (from IdentityUser)

        public decimal  price { get; set; }

        public Patient Patient { get; set; }


    }
}
