using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    public class ClinicAppointment: Appointment
    {
        public ClineAppointmentType IsOffile { get; set; } = ClineAppointmentType.offline;

        public string URL {  get; set; }


        public TimeOnly time {  get; set; } 


        public int ScheduleId { get; set; }
        //Navigation properties

        public Schedule Schedule { get; set; }





        public string PatientId { get; set; } // Assuming Patient.Id is a string (from IdentityUser)

        //naavigation property
        public Patient Patient { get; set; }



    }
}
