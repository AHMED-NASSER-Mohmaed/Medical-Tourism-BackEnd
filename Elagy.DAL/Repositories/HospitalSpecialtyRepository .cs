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
            Context = context;
        }

        public ApplicationDbContext Context { get; }

        public async Task<IEnumerable<HospitalSpecialty>> GetActiveHospitalSpecialtiesByHospitalIdAsync(string hospitalId)
        {
            return await _context.HospitalSpecialties
               .Where(hs => hs.HospitalAssetId == hospitalId)
               .Include(hs => hs.Specialty)
               .Include(hs => hs.HospitalAsset)
               .ToListAsync();
        }

        public async Task<HospitalSpecialty> GetByIdAsync(int id)
        {
            return await _context.HospitalSpecialties
                  .Include(hs => hs.Specialty)
                  .Include(hs => hs.HospitalAsset)
                  .FirstOrDefaultAsync(hs => hs.Id == id);

        }
    }
}
