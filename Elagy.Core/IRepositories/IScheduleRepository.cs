using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IScheduleRepository
    {
        Task<Schedule> GetByIdAsync(int id);
        Task<IEnumerable<Schedule>> GetAllAsync(); 
        Task AddAsync(Schedule schedule);
        Task UpdateAsync(Schedule schedule);
        Task SoftDeleteAsync(Schedule schedule);

    
        /// Retrieves schedules for a specific doctor on a given date, often for conflict checking.
        Task<IEnumerable<Schedule>> GetDoctorSchedulesOnDateAsync(string doctorId, DateTime date);

        /// Retrieves all schedules associated with a specific hospital.
        /// Includes related entities for DTO mapping.
        Task<IEnumerable<Schedule>> GetSchedulesByHospitalIdAsync(string hospitalId);

        /// Retrieves a single schedule by ID, ensuring it belongs to the specified hospital.
        /// Includes related entities for DTO mapping.
        Task<Schedule> GetScheduleByIdAndHospitalIdAsync(int id, string hospitalId);
    }
}
