using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Disbursement
{
    public class DisplayHospitalDisbursementItems 
    {
        //------Disbursement Item Details
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; } // Amount of the disbursement item

        //public int appointmentId { get; set; }
        //public int DisbursementId { get; set; }

        // -----Appointment details

        public Entities.SpecialtyAppointment Appointment{ get; set; } = new Entities.SpecialtyAppointment();

    }
}
