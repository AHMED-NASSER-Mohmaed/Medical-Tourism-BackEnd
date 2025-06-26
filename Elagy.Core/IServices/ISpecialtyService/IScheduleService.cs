using Elagy.Core.DTOs.DoctorSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices.ISpecialtyService
{
    public interface IScheduleService
    {
        /// <summary>
        /// Allows a Hospital Admin to assign a new schedule (time slot) to a doctor.
        /// Performs validation to ensure the doctor and hospital specialty are valid and associated with the admin's hospital.
        /// Checks for time conflicts for the doctor.
        /// </summary>
        Task<ScheduleDto> AssignScheduleByAdminAsync(CreateScheduleDto createDto, string hospitalAdminId);

        /// <summary>
        /// Retrieves all schedules for the authenticated hospital admin's hospital.
        /// </summary>
        Task<IEnumerable<ScheduleDto>> GetSchedulesForAdminAsync(string hospitalAdminId);

        /// <summary>
        /// Retrieves a specific schedule by ID for the authenticated hospital admin's hospital.
        /// </summary>
        Task<ScheduleDto> GetScheduleByIdForAdminAsync(int id, string hospitalAdminId);

        /// <summary>
        /// Updates an existing schedule.
        /// </summary>
        Task<bool> UpdateScheduleByAdminAsync(UpdateScheduleDto updateDto, string hospitalAdminId);

        /// <summary>
        /// Soft deletes a schedule by marking it as inactive.
        /// </summary>
        Task<bool> DeleteScheduleByAdminAsync(int id, string hospitalAdminId);
    }
}
