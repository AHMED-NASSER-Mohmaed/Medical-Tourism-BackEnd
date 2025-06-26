using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class SpecialtyAppointment : Appointment
    {
        /// <summary>
        /// Specifies whether the appointment is onsite (in-person) or online (virtual).
        /// </summary>
        public SpecialtyAppointmentDeliveryType DeliveryType { get; set; }
        /// <summary>
        /// The URL for the online meeting, if the DeliveryType is Online.
        /// This property is nullable.
        /// </summary>
        public string? MeetingUrl { get; set; }
    }
}
