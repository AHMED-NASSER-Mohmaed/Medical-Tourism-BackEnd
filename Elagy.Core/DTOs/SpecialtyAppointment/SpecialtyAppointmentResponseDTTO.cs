using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.SpecialtyAppointment
{
    public class SpecialtyAppointmentResponseDTTO
    {

        public int Id { get; set; }

        //if price changed at the clinic
        public decimal price { get; set; }

        //------ Hospital ---- Hotle ----- CarRental
        
        public AppointmentType Type { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        public Guid PackageId { get; set; }
        public SpecialtyAppointmentDeliveryType ServiceDeliveryType { get; set; } = SpecialtyAppointmentDeliveryType.Onsite;

        public string? MeetingUrl { get; set; }

        //calculated from the schedule Time SlotSize
        public TimeOnly ExistingTime { get; set; }
        public DateOnly Date { get; set; }

        public int SpecialtyScheduleId { get; set; }
        //Navigation properties

    }
}
