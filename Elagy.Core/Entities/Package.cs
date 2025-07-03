using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    //booking object
    public class Package
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PaymentConfirmedAt { get; set; }

        public string Currency { get; set; }
        public BookingStatus Status { get; set; }
        public string PatientId { get; set; } 
        //naavigation property
        public Patient Patient { get; set; }
        public string? StripeCheckoutSessionId { get; set; }
        public string? StripePaymentIntentId { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    }
}
