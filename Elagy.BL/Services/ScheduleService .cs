using AutoMapper;
using Elagy.Core.DTOs.DoctorSchedule;
using Elagy.Core.Entities;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices.ISpecialtyService;
using Elagy.DAL.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Elagy.BL.Services
{
    public class ScheduleService : IScheduleService
    {

    private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ScheduleService> _logger;
        private readonly IMapper _mapper;

        public ScheduleService(

            IUnitOfWork unitOfWork,
            ILogger<ScheduleService> logger,
            IMapper mapper)
        {

            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<ScheduleDto> AssignScheduleByAdminAsync(CreateScheduleDto createDto, string hospitalAdminId)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(createDto.DoctorId);
            if (doctor == null)
                throw new ArgumentException($"Doctor with ID '{createDto.DoctorId}' not found.");

            //var hospitalSpecialty = await _unitOfWork.HospitalSpecialties.GetByIdAsync(createDto.HospitalSpecialtyId);
            //if (hospitalSpecialty == null)
            //    throw new ArgumentException($"Hospital Specialty with ID '{createDto.HospitalSpecialtyId}' not found.");

            //if (doctor.HospitalSpecialtyId != createDto.HospitalSpecialtyId)
            //    throw new ArgumentException($"Doctor '{doctor.FirstName} {doctor.LastName}' is not primarily assigned to specialty '{hospitalSpecialty.Specialty?.Name}' at '{hospitalSpecialty.HospitalAsset?.Name}'.");

            //if (hospitalSpecialty.HospitalAssetId != hospitalAdminId)
            //    throw new UnauthorizedAccessException("You do not have permission to assign schedules for this hospital specialty.");

            if (createDto.StartTime >= createDto.EndTime)
                throw new ArgumentException("Schedule start time must be before end time.");

            var existingSchedules = await _unitOfWork.Schedules.GetDoctorSchedulesOnDateAsync(createDto.DoctorId, createDto.Date);
            bool hasConflict = existingSchedules.Any(s =>
                createDto.StartTime < s.EndTime && createDto.EndTime > s.StartTime
            );

            if (hasConflict)
                throw new ArgumentException("The proposed time slot conflicts with an existing schedule for this doctor on this date.");

            var schedule = new Schedule
            {
                DoctorId = createDto.DoctorId,
                HospitalSpecialtyId = createDto.HospitalSpecialtyId,
                Date = createDto.Date.Date,
                StartTime = createDto.StartTime,
                EndTime = createDto.EndTime,
                MaxCapacity = createDto.MaxCapacity,
                BookedSlots = 0,
                IsActive = createDto.IsActive
            };

            await _unitOfWork.Schedules.AddAsync(schedule);
            await _unitOfWork.CompleteAsync();

            var createdScheduleWithDetails = await _unitOfWork.Schedules.GetByIdAsync(schedule.Id);
            return _mapper.Map<ScheduleDto>(createdScheduleWithDetails);
        }

        public async Task<IEnumerable<ScheduleDto>> GetSchedulesForAdminAsync(string hospitalAdminId)
        {
            var schedules = await _unitOfWork.Schedules.GetSchedulesByHospitalIdAsync(hospitalAdminId);
            return _mapper.Map<IEnumerable<ScheduleDto>>(schedules);
        }

        public async Task<ScheduleDto> GetScheduleByIdForAdminAsync(int id, string hospitalAdminId)
        {
            var schedule = await _unitOfWork.Schedules.GetScheduleByIdAndHospitalIdAsync(id, hospitalAdminId);
            return _mapper.Map<ScheduleDto>(schedule);
        }

        public async Task<bool> UpdateScheduleByAdminAsync(UpdateScheduleDto updateDto, string hospitalAdminId)
        {
            var schedule = await _unitOfWork.Schedules.GetScheduleByIdAndHospitalIdAsync(updateDto.Id, hospitalAdminId);
            if (schedule == null) return false;

            Doctor updatedDoctor = schedule.Doctor;
            HospitalSpecialty updatedHospitalSpecialty = schedule.HospitalSpecialty;
            bool doctorChanged = false;
            bool hospitalSpecialtyChanged = false;

            if (updateDto.DoctorId != null && updateDto.DoctorId != schedule.DoctorId)
            {
                updatedDoctor = await _unitOfWork.Doctors.GetByIdAsync(updateDto.DoctorId);
                if (updatedDoctor == null) throw new ArgumentException($"Doctor with ID '{updateDto.DoctorId}' not found.");
                doctorChanged = true;
            }

            if (updateDto.HospitalSpecialtyId.HasValue && updateDto.HospitalSpecialtyId.Value != schedule.HospitalSpecialtyId)
            {
                //updatedHospitalSpecialty = await _unitOfWork.HospitalSpecialties.GetByIdAsync(updateDto.HospitalSpecialtyId.Value);
                if (updatedHospitalSpecialty == null) throw new ArgumentException($"Hospital Specialty with ID '{updateDto.HospitalSpecialtyId.Value}' not found.");
                hospitalSpecialtyChanged = true;
            }

            if (doctorChanged || hospitalSpecialtyChanged)
            {
                if (updatedDoctor.HospitalSpecialtyId != updatedHospitalSpecialty.Id)
                    throw new ArgumentException($"Doctor '{updatedDoctor.FirstName} {updatedDoctor.LastName}' is not primarily assigned to specialty '{updatedHospitalSpecialty.Specialty?.Name}' at '{updatedHospitalSpecialty.HospitalAsset?.Name}'.");

                if (updatedHospitalSpecialty.HospitalAssetId != hospitalAdminId)
                    throw new UnauthorizedAccessException("You do not have permission to assign schedules to this hospital specialty.");
            }

            schedule.DoctorId = updateDto.DoctorId ?? schedule.DoctorId;
            schedule.HospitalSpecialtyId = updateDto.HospitalSpecialtyId ?? schedule.HospitalSpecialtyId;
            schedule.Date = updateDto.Date?.Date ?? schedule.Date.Date;
            schedule.StartTime = updateDto.StartTime ?? schedule.StartTime;
            schedule.EndTime = updateDto.EndTime ?? schedule.EndTime;
            schedule.MaxCapacity = updateDto.MaxCapacity ?? schedule.MaxCapacity;
            schedule.IsActive = updateDto.IsActive ?? schedule.IsActive;

            if (updateDto.Date.HasValue || updateDto.StartTime.HasValue || updateDto.EndTime.HasValue || doctorChanged)
            {
                if (schedule.StartTime >= schedule.EndTime)
                    throw new ArgumentException("Schedule start time must be before end time.");

                var conflictingSchedules = await _unitOfWork.Schedules.GetDoctorSchedulesOnDateAsync(schedule.DoctorId, schedule.Date);
                bool hasConflict = conflictingSchedules.Any(s =>
                    s.Id != schedule.Id &&
                    s.IsActive &&
                    (schedule.StartTime < s.EndTime && schedule.EndTime > s.StartTime)
                );

                if (hasConflict)
                    throw new ArgumentException("The updated time slot conflicts with an existing active schedule for this doctor on this date.");
            }

            if (schedule.MaxCapacity < schedule.BookedSlots)
                throw new InvalidOperationException($"Cannot reduce max capacity to {schedule.MaxCapacity} as there are already {schedule.BookedSlots} booked appointments.");

            await _unitOfWork.Schedules.UpdateAsync(schedule);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteScheduleByAdminAsync(int id, string hospitalAdminId)
        {
            var schedule = await _unitOfWork.Schedules.GetScheduleByIdAndHospitalIdAsync(id, hospitalAdminId);
            if (schedule == null) return false;

            if (schedule.BookedSlots > 0)
                throw new InvalidOperationException($"Schedule {id} has booked appointments and cannot be deleted. Please cancel appointments first or deactivate the schedule instead.");

            await _unitOfWork.Schedules.SoftDeleteAsync(schedule);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
