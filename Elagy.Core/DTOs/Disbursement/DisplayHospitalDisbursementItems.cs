using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Disbursement
{
    class DisplayHospitalDisbursementItems : IDisplayDisbursementItems
    {
        //------Disbursement Item Details
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
        //public int PackageId { get; set; }
        //  public int DisbursementItemId { get; set; } // Foreign key to DisbursementItem

        // -----Appointement Speciality

        //(hospital)
        public SpecialtyAppointmentDeliveryType IsOffile { get; set; } = SpecialtyAppointmentDeliveryType.Onsite;
        public string? MeetingUrl { get; set; }
        public TimeOnly ExistingTime { get; set; }

        // public int ScheduleId { get; set; }
        //Navigation properties

        // ---- Specialty Schedule ----
        public string DoctorName { get; set; }
        //added by nasser ya mahdy

        //----HospitalSpecialty-----
        public string HospitalName { get; set; }


    }
}
