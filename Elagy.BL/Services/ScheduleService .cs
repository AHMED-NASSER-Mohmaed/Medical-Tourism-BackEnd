
using AutoMapper;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Schedule;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Elagy.BL.Services
{
    public class ScheduleService : ISpecialtyScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ScheduleService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

      
        /// Retrieves a paginated list of all schedules for a specific hospital's admin dashboard
        public async Task<PagedResponseDto<ScheduleResponseDto>> GetAllSchedulesForHospitalAdminAsync(string hospitalId, PaginationParameters paginationParameters)
        {
            try
            {
                
                var schedules = await _unitOfWork.Schedules.GetSchedulesByHospitalIdAsync(hospitalId, isActive: null);

            
                var query = schedules.AsQueryable();

                if (!string.IsNullOrWhiteSpace(paginationParameters.SearchTerm))
                {
                    string term = paginationParameters.SearchTerm.Trim().ToLower();
                    query = query.Where(s =>
                        s.Doctor.FirstName.ToLower().Contains(term) ||
                        s.Doctor.LastName.ToLower().Contains(term) ||
                        s.HospitalSpecialty.Specialty.Name.ToLower().Contains(term)
                    );
                }

                if (paginationParameters.FilterDayOfWeekId.HasValue)
                {
                    query = query.Where(s => s.DayOfWeekId == paginationParameters.FilterDayOfWeekId.Value);
                }

                if (paginationParameters.FilterIsActive.HasValue)
                {
                    query = query.Where(s => s.IsActive == paginationParameters.FilterIsActive.Value);
                }

                // added for filtering by doctors schedule price
                if (paginationParameters.MinPrice.HasValue)
                {
                    query = query.Where(s => s.Price >= paginationParameters.MinPrice.Value);
                }
                if (paginationParameters.MaxPrice.HasValue)
                {
                    query = query.Where(s => s.Price <= paginationParameters.MaxPrice.Value);
                }

                var totalCount = query.Count();

                
                var pagedSchedules = query
                    .OrderBy(s => s.DayOfWeekId)
                    .ThenBy(s => s.StartTime)
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToList();

                var scheduleDtos = _mapper.Map<IEnumerable<ScheduleResponseDto>>(pagedSchedules);

                return new PagedResponseDto<ScheduleResponseDto>(scheduleDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting schedules for Hospital Admin Dashboard for Hospital ID: {hospitalId}.");
                return new PagedResponseDto<ScheduleResponseDto>(Enumerable.Empty<ScheduleResponseDto>(), 0, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
        }

        public async Task<ScheduleResponseDto> CreateScheduleAsync(CreateScheduleSlotDto createDto, string hospitalId)
        {
            try
            {
            
                var doctor = await _unitOfWork.Doctors.GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(createDto.DoctorId);

                if (doctor == null) 
                    throw new ArgumentException($"Doctor with ID {createDto.DoctorId} not found.");
                if (doctor.HospitalSpecialty?.HospitalAssetId != hospitalId)
                    throw new UnauthorizedAccessException($"Doctor {createDto.DoctorId} is not affiliated with your hospital.");
                if (doctor.HospitalSpecialtyId != createDto.HospitalSpecialtyId)
                    throw new InvalidOperationException($"Doctor {createDto.DoctorId} is assigned to HS {doctor.HospitalSpecialtyId}, not {createDto.HospitalSpecialtyId}.");
                if (!doctor.HospitalSpecialty.IsActive) 
                    throw new InvalidOperationException($"Hospital Specialty {createDto.HospitalSpecialtyId} is not active.");
                if (doctor.Status != Status.Active) 
                    throw new InvalidOperationException($"Doctor {createDto.DoctorId} is not active and cannot have schedules created.");
                // 2. Validate DayOfWeek based on the Date
                if (createDto.DayOfWeekId < 1 || createDto.DayOfWeekId>7) 
                    throw new ArgumentException("Invalid DayOfWeekId provided.");

                // validate the start and end time and the estimated time for one patient
                if (createDto.EndTime <= createDto.StartTime) 
                    throw new ArgumentException("End time must be after start time.");
                if (createDto.TimeSlotSize <= 0) 
                    throw new ArgumentException("Time slot size must be a positive integer.");

                // convert to total minutes to calc the maxcapacity in the period
                var slotDurationMinutes = (createDto.EndTime - createDto.StartTime).TotalMinutes;
                // 3. Check for overlapping schedules for the same doctor on the same day (for active schedules)

                var existingSchedules = await _unitOfWork.Schedules.GetSchedulesByDoctorIdAsync(createDto.DoctorId, isActive: true);


                bool isOverlap = existingSchedules.Any(s =>
                        s.DayOfWeekId == createDto.DayOfWeekId && 
                        (// Same day of week
                        (createDto.StartTime < s.EndTime && createDto.EndTime > s.StartTime) || // Overlaps
                        (createDto.StartTime == s.StartTime && createDto.EndTime == s.EndTime) // Exact match
                ));

                if (isOverlap) 
                throw new InvalidOperationException("Doctor already has an overlapping schedule for the specified day and time.");

                // 4. Map DTO to Schedule entity
                var schedule = _mapper.Map<SpecialtySchedule>(createDto);


                // 5. Add to repository and persist
                await _unitOfWork.Schedules.AddAsync(schedule);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Schedule slot (ID: {schedule.Id}) created successfully for Doctor {createDto.DoctorId} at {createDto.HospitalSpecialtyId}.");


                var createdScheduleWithDetails = await _unitOfWork.Schedules.GetScheduleByIdWithDetailsAsync(schedule.Id);
                if (createdScheduleWithDetails == null) throw new Exception("Created schedule not found after save.");

                return _mapper.Map<ScheduleResponseDto>(createdScheduleWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating schedule for Hospital {hospitalId}.");
                throw;
            }
        }

        public async Task<ScheduleResponseDto> UpdateScheduleAsync(int scheduleId, UpdateScheduleDto updateDto, string hospitalId)
        {
            try
            {
                // 1. Retrieve the existing schedule with details
                var schedule = await _unitOfWork.Schedules.GetScheduleByIdWithDetailsAsync(scheduleId);

                if (schedule == null) throw new KeyNotFoundException($"Schedule with ID {scheduleId} not found for update.");

                // 2. Validate schedule's affiliation to the requesting hospital admin
                if (schedule.HospitalSpecialty.HospitalAssetId != hospitalId) 
                    throw new UnauthorizedAccessException($"Schedule with ID {scheduleId} is not affiliated with your hospital.");

                // 3. Handle specific updates based on DTO fields validation
                // MaxCapacity: Cannot be less than BookedSlots
                if (updateDto.MaxCapacity.HasValue && updateDto.MaxCapacity.Value < schedule.BookedSlots)
                {
                    throw new InvalidOperationException($"Max capacity ({updateDto.MaxCapacity.Value}) cannot be less than booked slots ({schedule.BookedSlots}).");
                }

                // 2. Validate DayOfWeek based on the Date
                if (updateDto.DayOfWeekId < 1 && updateDto.DayOfWeekId > 7)
                    throw new ArgumentException("Invalid DayOfWeekId provided.");




                // Overlap check if time/date/doctor changes
                if (( updateDto.StartTime.HasValue || updateDto.EndTime.HasValue))
                {
                    var proposedStartTime = updateDto.StartTime ?? schedule.StartTime;
                    var proposedEndTime = updateDto.EndTime ?? schedule.EndTime;
                    var proposedDayOfWeedId = updateDto.DayOfWeekId ?? schedule.DayOfWeekId;



                    // Check for overlaps with other active schedules of the same doctor
                    var existingSchedules = await _unitOfWork.Schedules.GetSchedulesByDoctorIdAsync(schedule.DoctorId, isActive: true);
                    bool isOverlap = existingSchedules.Any(s =>
                        s.Id != schedule.Id && // Exclude current schedule itself
                        s.DayOfWeekId == proposedDayOfWeedId && // Same day of week
                        (
                            (proposedStartTime < s.EndTime && proposedEndTime > s.StartTime) || // Overlaps
                            (proposedStartTime == s.StartTime && proposedEndTime == s.EndTime) // Exact match
                        )
                    );
                    if (isOverlap) throw new InvalidOperationException("Updated schedule time/date overlaps with an existing schedule for this doctor.");
                }

                // 4. Map remaining DTO properties to the entity
                // This maps Date, StartTime, EndTime, MaxCapacity, IsActive, IsRecurring
                _mapper.Map(updateDto, schedule);

                // 5. Update the entity in the repository and save changes
                _unitOfWork.Schedules.Update(schedule);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Schedule slot (ID: {schedule.Id}) updated successfully for Doctor {schedule.DoctorId}.");

                // 6. Re-fetch the updated schedule with details for comprehensive response DTO
                var updatedScheduleWithDetails = await _unitOfWork.Schedules.GetScheduleByIdWithDetailsAsync(scheduleId);
                if (updatedScheduleWithDetails == null) throw new Exception("Updated schedule not found after save.");

                return _mapper.Map<ScheduleResponseDto>(updatedScheduleWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating schedule ID: {scheduleId} for Hospital {hospitalId}.");
                throw;
            }
        }
        public async Task<ScheduleResponseDto?> ChangeScheduleStatusAsync(int scheduleId, bool newIsActiveStatus, string hospitalId)
        {
            try
            {
                // 1. Retrieve the existing schedule with details (to check affiliation and current status)
                var schedule = await _unitOfWork.Schedules.GetScheduleByIdWithDetailsAsync(scheduleId);
                if (schedule == null) throw new KeyNotFoundException($"Schedule with ID {scheduleId} not found for status change.");

                // 2. Validate schedule's affiliation to the requesting hospital admin
                if (schedule.HospitalSpecialty.HospitalAssetId != hospitalId) throw new UnauthorizedAccessException($"Schedule with ID {scheduleId} is not affiliated with your hospital.");

                // 3. If the status is already the target status, no action needed.
                if (schedule.IsActive == newIsActiveStatus)
                {
                    _logger.LogInformation($"Schedule with ID {scheduleId} is already in the target status ({newIsActiveStatus}). No action needed.");
                    return _mapper.Map<ScheduleResponseDto>(schedule);
                }

                // 4. Update the IsActive status
                schedule.IsActive = newIsActiveStatus;
                _unitOfWork.Schedules.Update(schedule); // Mark as modified
                await _unitOfWork.CompleteAsync(); // Save changes

                _logger.LogInformation($"Schedule '{scheduleId}' status changed to {newIsActiveStatus} successfully for Hospital {hospitalId}.");

                // 5. Re-fetch for full DTO response (to ensure navigation properties are loaded)
                var updatedScheduleWithDetails = await _unitOfWork.Schedules.GetScheduleByIdWithDetailsAsync(scheduleId);
                if (updatedScheduleWithDetails == null) throw new Exception("Updated schedule not found after save.");

                return _mapper.Map<ScheduleResponseDto>(updatedScheduleWithDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing status for schedule ID: {scheduleId} for Hospital {hospitalId}.");
                throw;
            }
        }

        public async Task<PagedResponseDto<ScheduleResponseDto>> GetAvailablePatientSlotsAsync(PaginationParameters paginationParameters)
        {
            try
            {
                // 1. Start with an IQueryable from the repository for all schedules
                var query = _unitOfWork.Schedules.AsQueryable();

                // 2. Filter for truly available slots (active and has capacity)
                query = query.Where(s => s.IsActive == true && s.BookedSlots < s.MaxCapacity);

                // 3. Apply optional filters from PaginationParameters

                // Filter by Specialty (SpecialtyId) - requires Include
                if (paginationParameters.SpecialtyId.HasValue) // Using SpecialtyId from PaginationParameters
                {
                    query = query.Where(s => s.HospitalSpecialty.SpecialtyId == paginationParameters.SpecialtyId.Value);
                }
                // Filter by DoctorId
                if (!string.IsNullOrWhiteSpace(paginationParameters.FilterDoctorId)) // Using FilterDoctorId from PaginationParameters
                {
                    query = query.Where(s => s.DoctorId == paginationParameters.FilterDoctorId);
                }
                // Filter by DayOfWeekId
                if (paginationParameters.FilterDayOfWeekId.HasValue)
                {
                    query = query.Where(s => s.DayOfWeekId == paginationParameters.FilterDayOfWeekId.Value);
                }
                 
                // 4. Get total count AFTER applying filters
                var totalCount = await query.CountAsync();

                // 5. Apply pagination and eager load navigation properties for DTO mapping
                var pagedSchedules = await query
                    .Include(s => s.Doctor) // Needed for DoctorName, Email
                    .Include(s => s.HospitalSpecialty) // Needed for HospitalName, SpecialtyName
                        .ThenInclude(hs => hs.HospitalAsset)
                    .Include(s => s.HospitalSpecialty)
                        .ThenInclude(hs => hs.Specialty)
                    .Include(s => s.DayOfWeek) // Needed for DayOfWeekName, ShortCode
                    .OrderBy(s => s.StartTime)
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                // 6. Map to DTOs
                var scheduleDtos = _mapper.Map<IEnumerable<ScheduleResponseDto>>(pagedSchedules);

                return new PagedResponseDto<ScheduleResponseDto>(scheduleDtos, totalCount, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available patient slots.");
                return new PagedResponseDto<ScheduleResponseDto>(Enumerable.Empty<ScheduleResponseDto>(), 0, paginationParameters.PageNumber, paginationParameters.PageSize);
            }
        }

        /// Retrieves a single schedule slot by its ID for detail view.
 
        public async Task<ScheduleResponseDto?> GetScheduleByIdAsync(int scheduleId)
        {
            try
            {
                var schedule = await _unitOfWork.Schedules.GetScheduleByIdWithDetailsAsync(scheduleId);
                if (schedule == null)
                {
                    _logger.LogInformation($"Schedule with ID {scheduleId} not found.");
                    return null;
                }
                return _mapper.Map<ScheduleResponseDto>(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting schedule by ID: {scheduleId}.");
                throw;
            }
        }
    }
}