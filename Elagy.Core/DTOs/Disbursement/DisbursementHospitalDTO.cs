using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.DTOs.Disbursement
{
    public class DisbursementHospitalDTO
    {
        public int Id { get; set; }
        public DateOnly DisbursementDateMonth { get; set; }
        public DateTime GeneratedAt { get; set; }
        public decimal TotalAmount { get; set; }
        public List<DisbursementItemDto> DisbursementItems { get; set; }
    }

    public class DisbursementItemDto
    {
        public int Id { get; set; }
        public AppointmentDto Appointment { get; set; }
    }

    public class AppointmentDto
    {
        public int Id { get; set; }
        public decimal price { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public SpecialtyScheduleDto SpecialtySchedule { get; set; }
    }

    public class SpecialtyScheduleDto
    {
        public int Id { get; set; }
        
        public string Specialty { get; set; } // Specialty name
        public DoctorDto Doctor { get; set; }
    }

    public class DoctorDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

}
