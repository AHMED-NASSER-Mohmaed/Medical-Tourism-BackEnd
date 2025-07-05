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
    public class PackageResponseDTO
    {
        public Guid Id{ get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PaymentConfirmedAt { get; set; }//when hock is called and payment is confirmed

        public decimal TatalAmount { get; set; }
        public BookingStatus Status { get; set; }

        public string PatientId { get; set; }

        public SpecialtyAppointmentResponseDTTO SpecialtyAppoinment{ get; set; } 

    }
}
