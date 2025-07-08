using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Disbursement
{
    public class DisplayCarRentalDisbursementItems
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; } // Amount of the disbursement item

        //public int appointmentId { get; set; }
        //public int DisbursementId { get; set; }

        // -----Appointment details

        public decimal price { get; set; }

        //------ Hospital ---- Hotle ----- CarRental
        public AppointmentType Type { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        //(CarRental)------------
        public DateTime StartingDateTime { get; set; }
        public DateTime EndingDateTime { get; set; }

    }
}
