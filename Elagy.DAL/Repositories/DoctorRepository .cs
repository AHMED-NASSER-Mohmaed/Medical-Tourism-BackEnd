using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.DAL.Repositories
{
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext context) : base(context) { }


         private IQueryable<Doctor> GetDoctorsWithAllDetails()
        {
            return _dbSet
                .Include(d => d.Governorate)
                    .ThenInclude(g => g.Country) 
                .Include(d => d.HospitalSpecialty)
                    .ThenInclude(hs => hs.HospitalAsset)
                .Include(d => d.HospitalSpecialty)
                    .ThenInclude(hs => hs.Specialty);
        }
     
        public async Task<IEnumerable<Doctor>> GetDoctorsByHospitalIdAsync(string hospitalId, bool isActive = true)
        {
            var query = GetDoctorsWithAllDetails()
                .Where(d => d.HospitalSpecialty.HospitalAssetId == hospitalId);

            //if (isActive)
            //{
            //    query = query.Where(d => d.Status == Status.Active);
            //}
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByHospitalSpecialtyIdAsync(int hospitalSpecialtyId, bool isActive = true)
        {
            var query = GetDoctorsWithAllDetails()
                .Where(d => d.HospitalSpecialtyId == hospitalSpecialtyId);

            if (isActive)
            {
                query = query.Where(d => d.Status == Status.Active);
            }
            return await query.ToListAsync();
        }

        public async Task<Doctor?> GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(string doctorId)
        {
            return await GetDoctorsWithAllDetails()
                .FirstOrDefaultAsync(d => d.Id == doctorId);
        }
    }
}
