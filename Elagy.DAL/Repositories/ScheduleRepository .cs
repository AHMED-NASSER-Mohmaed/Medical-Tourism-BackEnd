using Elagy.Core.Entities;
using Elagy.Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.DAL.Repositories
{
    public class ScheduleRepository : GenericRepository<SpecialtySchedule>, IScheduleRepository
    {
       
        public ScheduleRepository(ApplicationDbContext _context):base(_context) { }

        private IQueryable<SpecialtySchedule> GetSchedulesWithAllDetails()
        {
            return _dbSet
                .Include(s => s.Doctor)
                .Include(s => s.HospitalSpecialty)
                    .ThenInclude(hs => hs.HospitalAsset)
                .Include(s => s.HospitalSpecialty)
                    .ThenInclude(hs => hs.Specialty)
                .Include(s => s.DayOfWeek);
        }
        public async Task<SpecialtySchedule?> GetScheduleByIdWithDetailsAsync(int scheduleId)
        {
            return await GetSchedulesWithAllDetails().FirstOrDefaultAsync(s => s.Id == scheduleId);
        }

        public async Task<IEnumerable<SpecialtySchedule>> GetSchedulesByDoctorIdAsync(string doctorId, bool? isActive = null)
        {
            var query = GetSchedulesWithAllDetails().Where(s => s.DoctorId == doctorId);

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<SpecialtySchedule>> GetSchedulesByHospitalIdAsync(string hospitalId, bool? isActive = null)
        {
            var query = GetSchedulesWithAllDetails() 
                .Where(s => s.HospitalSpecialty.HospitalAssetId == hospitalId); 

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value); 
            }

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<SpecialtySchedule>> GetSchedulesByHospitalSpecialtyIdAsync(int hospitalSpecialtyId, bool? isActive = null)
        {
            var query = GetSchedulesWithAllDetails()
                .Where(s => s.HospitalSpecialtyId == hospitalSpecialtyId);

            if (isActive.HasValue)
            {
                query = query.Where(s => s.IsActive == isActive.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<bool> UpdateScheduleStatusAsync(int scheduleId, bool newIsActiveStatus)
        {
            var schedule = await _dbSet.FirstOrDefaultAsync(s => s.Id == scheduleId);
            if (schedule == null)
            {
                return false; 
            }

            if (schedule.IsActive == newIsActiveStatus)
            {
                return true; 
            }

            schedule.IsActive = newIsActiveStatus;
            _context.Entry(schedule).State = EntityState.Modified; 
          
            return true;
        }

        public async Task<IEnumerable<SpecialtySchedule>> GetAvailableSchedulesByDoctorIdAsync(string doctorId)
        {
            var query = GetSchedulesWithAllDetails()
                .Where(s => s.DoctorId == doctorId && s.IsActive == true);

            return await query.ToListAsync();
        }

     
    }
}
