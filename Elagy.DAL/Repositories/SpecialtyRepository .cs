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
    public class SpecialtyRepository : GenericRepository<Specialty>, ISpecialtyRepository
    {
        protected readonly ApplicationDbContext _context;
        public SpecialtyRepository(ApplicationDbContext _context) : base(_context) { }

        public async Task<Specialty> GetSpecialtyIdAsync(int id)
        {
            return await _context.Specialties
                .Where(s => s.Id == id && s.IsActive)
                .Include(s => s.HospitalSpecialties)
                .ThenInclude(hs => hs.HospitalAsset)
                .FirstOrDefaultAsync();
        }
        // for admin and website (true in case of website)
        public async Task<IEnumerable<Specialty>> GetSpecialtiesByHospitalIdAsync(string hospitalId, bool isActive = true)
        {
            var query = _context.Specialties
                .Include(s => s.HospitalSpecialties)
                .Where(s => s.HospitalSpecialties.Any(hs => hs.HospitalAssetId == hospitalId));

            if (isActive)
            {
                query = query.Where(s => s.IsActive);
            }

            return await query.ToListAsync();
        }
        // for super admin dashboard
        public async Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync(bool isActive = true)
        {
            var query = _context.Specialties.AsQueryable();

            if (isActive)
            {
                query = query.Where(s => s.IsActive);
            }

            return await query.ToListAsync();
        }
        //for hospital admin to link specilities
        public async Task<IEnumerable<Specialty>> GetUnlinkedSpecialtiesForHospitalAsync(string hospitalId)
        {
            var linkedSpecialtyIds = await _context.HospitalSpecialties.Include(s=>s.Specialty)
                .Where(hs => hs.HospitalAssetId == hospitalId  )
                .Select(hs => hs.SpecialtyId)
                .ToListAsync();

            var query = _context.Specialties
                .Where(s => !linkedSpecialtyIds.Contains(s.Id));
            
                query = query.Where(s => s.IsActive);
            

            return await query.ToListAsync();
        }
    }
}
