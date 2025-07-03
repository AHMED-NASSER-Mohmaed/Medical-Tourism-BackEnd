using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public abstract class Appointment
    {
        public int Id { get; set; }

        //if price changed at the clinic
        public decimal  price { get; set; }

        //------ Hospital ---- Hotle ----- CarRental
        public AppointmentType Type { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;



        public int PackageId { get; set; } 
        public Package Package { get; set; } 

    }
}
