using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class SpecialtyAppointment: Appointment
    {
        public SpecialtyAppointmentDeliveryType ServiceDeliveryType { get; set; } = SpecialtyAppointmentDeliveryType.Onsite;

        public string? MeetingUrl { get; set; }

        //calculated from the schedule Time SlotSize
        public TimeOnly ExistingTime {  get; set; } 
        public DateOnly Date {  get; set; } 

        public int SpecialtyScheduleId { get; set; }
        //Navigation properties

        public SpecialtySchedule SpecialtySchedule { get; set; }

    }
}
