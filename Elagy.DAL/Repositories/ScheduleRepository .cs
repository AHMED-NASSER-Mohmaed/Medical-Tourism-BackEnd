using Elagy.Core.Entities;
using Elagy.Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.DAL.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ApplicationDbContext _context;
        public ScheduleRepository(ApplicationDbContext _context)
        {
            this._context = _context;
        }
        public async Task AddAsync(Schedule schedule)
        {
            _context.Schedules.AddAsync(schedule);
        }

        public async Task<IEnumerable<Schedule>> GetAllAsync()
        {
            return await _context.Schedules
                 .Include(s => s.Doctor)
                     .ThenInclude(d => d.HospitalSpecialty)
                         .ThenInclude(hs => hs.HospitalAsset)
                 .Include(s => s.HospitalSpecialty)
                     .ThenInclude(hs => hs.Specialty)
                 .ToListAsync();
        }

        public async Task<Schedule> GetByIdAsync(int id)
        {
            return await _context.Schedules
              .Include(s => s.Doctor)
                  .ThenInclude(d => d.HospitalSpecialty)
                      .ThenInclude(hs => hs.HospitalAsset)
              .Include(s => s.HospitalSpecialty)
                  .ThenInclude(hs => hs.Specialty)
              .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Schedule>> GetDoctorSchedulesOnDateAsync(string doctorId, DateTime date)
        {
            return await _context.Schedules
                .Where(s => s.DoctorId == doctorId && s.Date.Date == date.Date && s.IsActive)
                .ToListAsync();
        }

        public async Task<Schedule> GetScheduleByIdAndHospitalIdAsync(int id, string hospitalId)
        {
            return await _context.Schedules
                 .Where(s => s.Id == id && s.HospitalSpecialty.HospitalAssetId == hospitalId)
                 .Include(s => s.Doctor)
                     .ThenInclude(d => d.HospitalSpecialty)
                         .ThenInclude(hs => hs.HospitalAsset)
                 .Include(s => s.HospitalSpecialty)
                     .ThenInclude(hs => hs.Specialty)
                 .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Schedule>> GetSchedulesByHospitalIdAsync(string hospitalId)
        {
            return await _context.Schedules
             .Where(s => s.HospitalSpecialty.HospitalAssetId == hospitalId)
             .Include(s => s.Doctor)
                 .ThenInclude(d => d.HospitalSpecialty) // Include necessary for Doctor properties
                     .ThenInclude(hs => hs.HospitalAsset) // Include if HospitalAsset details needed directly from Doctor path
             .Include(s => s.HospitalSpecialty)
                 .ThenInclude(hs => hs.Specialty) // Include Specialty name for DTO
             .OrderBy(s => s.Date) // Order for consistent display
             .ThenBy(s => s.StartTime)
             .ToListAsync();
        }

        public async Task SoftDeleteAsync(Schedule schedule)
        {
            schedule.IsActive = false;
            _context.Schedules.Update(schedule);
        }

        public async Task UpdateAsync(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
        }
    }
}
