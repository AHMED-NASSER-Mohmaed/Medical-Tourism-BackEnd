using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.SpecialtyAppointment
{
    public class CreateSpecialtyAppointmentDTO
    {
        public int SpecialtyScheduleId { get; set; }
        public bool isOffline { get; set; }
        public DateOnly AppointmentDate { get; set; }
    }
}
