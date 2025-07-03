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
     
        public SpecialtyRepository(ApplicationDbContext _context) : base(_context) { }

        private IQueryable<Specialty> GetSpecialtiesWithAllDetails()
        {
            return _dbSet 
                .Include(s => s.HospitalSpecialties)
                    .ThenInclude(hs => hs.HospitalAsset);
    
        }



        public async Task<Specialty?> GetSpecialtyIdAsync(int id)
        {
           return await GetSpecialtiesWithAllDetails()
                .FirstOrDefaultAsync(s => s.Id == id ); 
        }
        // for admin and website (true in case of website)
        public async Task<IEnumerable<Specialty>> GetSpecialtiesByHospitalIdAsync(string hospitalId)
        {
            var query = GetSpecialtiesWithAllDetails()
                     .Where(s => s.HospitalSpecialties.Any(hs => hs.HospitalAssetId == hospitalId));

            return await query.ToListAsync();




        }
        // for super admin dashboard
        public async Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync()
        {
            return await GetSpecialtiesWithAllDetails()
                .OrderBy(s => s.Name) // Added default order for consistency
                .ToListAsync();
        }
        //for hospital admin to link specilities
        public async Task<IEnumerable<Specialty>> GetUnlinkedSpecialtiesForHospitalAsync(string hospitalId)
        {
            var linkedSpecialtyIds = await _context.HospitalSpecialties
              .Where(hs => hs.HospitalAssetId == hospitalId)
              .Select(hs => hs.SpecialtyId)
              .ToListAsync();

            return await GetSpecialtiesWithAllDetails()
                .Where(s => !linkedSpecialtyIds.Contains(s.Id) && s.IsActive)
                .ToListAsync();
        }
    
    }
}
