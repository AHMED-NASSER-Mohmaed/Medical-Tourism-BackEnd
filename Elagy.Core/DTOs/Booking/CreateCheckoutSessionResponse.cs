using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Booking
{
    // --- DTO for the response from creating Stripe Checkout Session (remains similar) ---
    public class CreateCheckoutSessionResponse
    {
        public string CheckoutSessionUrl { get; set; }
    }
}
