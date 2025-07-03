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
    public class HospitalSpecialtyRepository : GenericRepository<HospitalSpecialty>, IHospitalSpecialtyRepository
    {
        public HospitalSpecialtyRepository(ApplicationDbContext context) : base(context)
        {
           
        }

       

        public async Task<HospitalSpecialty?> GetByHospitalAndSpecialtyIdAsync(string hospitalId, int specialtyId, Func<IQueryable<HospitalSpecialty>, IQueryable<HospitalSpecialty>>? includes = null)
        {
            var query = _dbSet
         .Where(hs => hs.HospitalAssetId == hospitalId && hs.SpecialtyId == specialtyId);

            if (includes != null)
            {
                query = includes(query);
            }

            return await query.FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<HospitalSpecialty>> GetByHospitalIdAsync(string hospitalId)
        {
            return await _context.HospitalSpecialties
                .Include(hs => hs.Specialty)
                .Where(hs => hs.HospitalAssetId == hospitalId)
                .ToListAsync();
        }

        public async Task<HospitalSpecialty?> GetByIdWithDetailsAsync(int hospitalspecialtyid)
        {
            return await _dbSet 
               .Where(hs => hs.Id == hospitalspecialtyid)
               .Include(hs => hs.Specialty) 
               .Include(hs => hs.HospitalAsset) 
               .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateHospitalSpecialtyLinkStatusAsync(
      string hospitalId,
      int specialtyId,
      bool newIsActiveStatus)
        {
            var link = await _dbSet
                .FirstOrDefaultAsync(hs =>
                    hs.HospitalAssetId == hospitalId &&
                    hs.SpecialtyId == specialtyId);

            if (link == null) return false;

     
            if (link.IsActive == newIsActiveStatus) return true;

            link.IsActive = newIsActiveStatus;

            return true;
        }
    }
}
