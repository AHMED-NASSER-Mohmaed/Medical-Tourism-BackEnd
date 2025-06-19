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
    public class DoctorRepository : GenericRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsWithHospitalSpecialtyAndSpecialtyAsync()
        {
            return await _dbSet.Include(d => d.HospitalSpecialty)
                               .ThenInclude(hs => hs.Specialty)
                              .Include(d => d.HospitalSpecialty)
                               .ThenInclude(hs => hs.HospitalAsset)
                              .ToListAsync();
        }

        public async Task<Doctor> GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(string doctorId)
        {
            return await _dbSet.Where(d => d.Id == doctorId)
                             .Include(d => d.HospitalSpecialty)
                                .ThenInclude(hs => hs.Specialty) // For Doctor's specific Specialty name
                             .Include(d => d.HospitalSpecialty)
                                .ThenInclude(hs => hs.HospitalAsset) // For Hospital details (e.g., name for display)
                             .FirstOrDefaultAsync();
        }

        public async Task<Doctor> GetDoctorIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Doctor>> GetDoctorsByHospitalSpecialtyAsync(int hospitalSpecialtyId)
        {
            return await _dbSet.Where(d => d.HospitalSpecialtyId == hospitalSpecialtyId)
                                  .Include(d => d.HospitalSpecialty) // Must include to access nested Specialty/HospitalAsset
                                     .ThenInclude(hs => hs.Specialty)
                                  .Include(d => d.HospitalSpecialty)
                                     .ThenInclude(hs => hs.HospitalAsset)
                                  .ToListAsync();
        }
    }
}
