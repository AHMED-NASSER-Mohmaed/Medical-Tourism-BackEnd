using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class PaymentIntent
    {

        public int Id { get; set; }


        // Stripe Identifiers
        public string StripePaymentIntentId { get; set; }
        public string StripeChargeId { get; set; }
        public string StripeCustomerId { get; set; }
        public string StripeInvoiceId { get; set; }
        public string StripePaymentMethodId { get; set; }

        // Payment Details
        public long Amount { get; set; } // in cents
        public string Currency { get; set; }
        public string PaymentStatus { get; set; }
        public string ReceiptUrl { get; set; }
        public string Description { get; set; }

        // Card Info
        public string PaymentMethodType { get; set; }
        public string CardBrand { get; set; }
        public string CardLast4 { get; set; }
        public int? CardExpMonth { get; set; }
        public int? CardExpYear { get; set; }

        // Additional Flags
        public bool IsCaptured { get; set; }
        public DateTime? CapturedAt { get; set; }
        public bool Refunded { get; set; }
        public long RefundedAmount { get; set; }
        public string FailureMessage { get; set; }

        // Stripe Metadata (Optional JSON)
        public string StripeRawDataJson { get; set; }

        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;





        //forging key to the package
        public int PackageId{ get; set; } 
        public Package Packages{ get; set; } 
    }
}
