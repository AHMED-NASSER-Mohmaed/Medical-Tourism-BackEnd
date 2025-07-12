using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.SpecialtyAppointment
{
    public class HospitalAppointmentDto
    {
        public int AppointmentId { get; set; }
        public DateOnly Date { get; set; }
        public TimeSpan Time { get; set; }
        public AppointmentStatus Status { get; set; }

        public string PatientName { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhone { get; set; }
        public string PatientCountry { get; set; }

        public string DoctorId { get; set; }
        public string DoctorName { get; set; }

        public string Specialty { get; set; }
    }
}
