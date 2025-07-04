using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Booking
{
    public class CreatePendingBookingResponse
    {
        public int BookingId { get; set; } // Your internal unique ID for this booking
        public decimal TotalAmount { get; set; } // The calculated total price
        public string Currency { get; set; } = "usd"; // The currency used
        public string BookingTitle { get; set; } // A summary title
        public string CustomerEmail { get; set; } // Pass through for Stripe Checkout
    }
}
