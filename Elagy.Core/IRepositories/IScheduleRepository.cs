using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IScheduleRepository: IGenericRepository<Schedule>
    {
        // Get schedules for a specific doctor
        Task<IEnumerable<Schedule>> GetSchedulesByDoctorIdAsync(string doctorId, bool? isActive = null);
        //Retrieves schedules associated with a specific hospital
        Task<IEnumerable<Schedule>> GetSchedulesByHospitalIdAsync(string hospitalId, bool? isActive = null);

        // Get schedules for a specific hospital specialty (clinic)
        Task<IEnumerable<Schedule>> GetSchedulesByHospitalSpecialtyIdAsync(int hospitalSpecialtyId, bool? isActive = null);

        // Get schedules with all related details (Doctor, HospitalSpecialty, DayOfWeek)
        Task<Schedule?> GetScheduleByIdWithDetailsAsync(int scheduleId);
        Task<bool> UpdateScheduleStatusAsync(int scheduleId, bool newIsActiveStatus);
    }
}
