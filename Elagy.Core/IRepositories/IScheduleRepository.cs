using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IScheduleRepository: IGenericRepository<SpecialtySchedule>
    {
        // Get schedules for a specific doctor
        Task<IEnumerable<SpecialtySchedule>> GetSchedulesByDoctorIdAsync(string doctorId, bool? isActive = null);
        //Retrieves schedules associated with a specific hospital
        Task<IEnumerable<SpecialtySchedule>> GetSchedulesByHospitalIdAsync(string hospitalId, bool? isActive = null);

        // Get schedules for a specific hospital specialty (clinic)
        Task<IEnumerable<SpecialtySchedule>> GetSchedulesByHospitalSpecialtyIdAsync(int hospitalSpecialtyId, bool? isActive = null);

        // Get schedules with all related details (Doctor, HospitalSpecialty, DayOfWeek)
        Task<SpecialtySchedule?> GetScheduleByIdWithDetailsAsync(int scheduleId);
        Task<bool> UpdateScheduleStatusAsync(int scheduleId, bool newIsActiveStatus);

        Task<IEnumerable<Schedule>> GetAvailableSchedulesByDoctorIdAsync(string doctorId);
    }
}
