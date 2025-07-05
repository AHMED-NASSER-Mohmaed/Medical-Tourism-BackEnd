using Elagy.Core.DTOs.SpecialtyAppointment;
using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface ISpecialtyAppointmentService
    {
        Task<Package> BookAppointment(string PatientId,CreateSpecialtyAppointmentDTO _);
        Task<(bool IsAvailable,int AppointmentCount, SpecialtySchedule SS)> IsAvailableAppointmentForBooking(DateOnly _ ,int SpecialtyScheduleId);


    }
}
