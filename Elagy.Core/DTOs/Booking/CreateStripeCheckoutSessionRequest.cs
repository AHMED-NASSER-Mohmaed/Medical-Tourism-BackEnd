using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Booking
{
    // --- DTO for the request to create Stripe Checkout Session (after pending booking is created) ---
    public class CreateStripeCheckoutSessionRequest
    {
        public Guid BookingId { get; set; } // The ID of the pending booking you just created
    }
}
