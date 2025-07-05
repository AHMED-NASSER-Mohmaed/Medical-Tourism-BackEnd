using Elagy.Core.DTOs.SpecialtyAppointment;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Package
{
    public class CreatePackageDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? PaymentConfirmedAt { get; set; }
        public BookingStatus Status { get; set; }
        public string PatientId { get; set; }
        //naavigation property
        public Patient Patient { get; set; }
        public SpecialtyAppointmentResponseDTTO SpecialtyAppoinment { get; set; }


        //public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
