using Elagy.Core.DTOs.CarAppointment;
using Elagy.Core.DTOs.RoomAppoinment;
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
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? PaymentConfirmedAt { get; set; }//when hock is called and payment is confirmed

        public decimal TatalAmount { get
            {
                decimal total = 0;
                if (SpecialtyAppoinment != null)
                    total += SpecialtyAppoinment.price;
                if (RoomAppointment != null)
                    total += RoomAppointment.price;
                if (CarAppointment != null)
                    total += CarAppointment.price;
                return total;
            } 
        }

        public bool CanCancel
        {
            get
            {
                var toDday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);

                if (this.SpecialtyAppoinment.Date > DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1)
                    || ( this.RoomAppointment!=null && this.RoomAppointment.CheckInDate > toDday ) 
                    || ( this.CarAppointment!=null && this.CarAppointment.StartingDate > toDday)
                    )
                    return  false;

                return true;
            }

        } //if the package can be cancelled or not}

        public BookingStatus Status { get; set; }

        public string PatientId { get; set; }


        public SpecialtyAppointmentResponseDTTO SpecialtyAppoinment { get; set; }

        public RoomAppointmentResponseDTO? RoomAppointment { get; set; } 

        public CarAppointmentResponseDTO? CarAppointment { get; set; } //if any car appointment is booked
    }
}