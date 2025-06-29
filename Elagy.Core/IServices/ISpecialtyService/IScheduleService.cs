using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices.ISpecialtyService
{
    public interface IScheduleService
    {
        //Retrieves list of all schedules hospital's admin dashboard
        Task<PagedResponseDto<ScheduleResponseDto>> GetAllSchedulesForHospitalAdminAsync(string hospitalId, PaginationParameters paginationParameters);
    
        //Creates a new schedule slot for a doctor within a specific hospital specialty.
        Task<ScheduleResponseDto> CreateScheduleAsync(CreateScheduleSlotDto createDto, string hospitalId);

        Task<ScheduleResponseDto> UpdateScheduleAsync(int scheduleId, UpdateScheduleDto updateDto, string hospitalId);
     
        /// Active and Deactive 

        Task<ScheduleResponseDto?> ChangeScheduleStatusAsync(int scheduleId, bool newIsActiveStatus, string hospitalId);

        // --- Public/Website Schedule Display ---


        /// Retrieves a paginated list of available schedule slots for patient viewing (website).

        Task<PagedResponseDto<ScheduleResponseDto>> GetAvailablePatientSlotsAsync(PaginationParameters paginationParameters);

        /// Retrieves a single schedule slot by its ID, typically for detail view.
        Task<ScheduleResponseDto?> GetScheduleByIdAsync(int scheduleId);
    }
}
