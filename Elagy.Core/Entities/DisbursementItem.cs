using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class DisbursementItem
    {
        public int Id { get; set; } // Unique identifier for the disbursement item
        public int DisbursementId { get; set; } // Foreign key to the Disbursement entity
        public Disbursement Disbursement { get; set; } // Navigation property to the Disbursement entity



        public decimal Amount { get; set; } // Amount of the disbursement item
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Timestamp when the item was created



        public int AppointmentId { get; set; } // Foreign key to the Appointment entity

        public Appointment Appointment { get; set; } // Navigation property to the Appointment entity

    }
}
