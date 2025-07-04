using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Enums
{
    // Interfaces/Models for your internal booking system
    public enum BookingStatus
    {
        PendingPayment,
        Confirmed,
        Canceled,
        PaymentFailed
    }
}
