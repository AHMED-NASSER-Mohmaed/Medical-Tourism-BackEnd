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
    public class HospitalRepository : GenericRepository<HospitalAsset>, IHospitalRepository
    {
        public HospitalRepository(ApplicationDbContext context) : base(context) { }

        public async Task<HospitalAsset> GetHospitalIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<HospitalSpecialty>> GetHospitalSpecialtiesInHospitalAsync(string hospitalId)
        {
            return await _context.HospitalSpecialties 
                .Where(hs => hs.HospitalAssetId == hospitalId)
                .Include(hs => hs.Specialty) 
                .Include(hs => hs.HospitalAsset) 
                .ToListAsync();

        }


        public async Task<IQueryable<HospitalAsset>> GetHospitalsBySpecialtyQueryAsync(int specialtyId)
        {
            return _context.HospitalSpecialties 
                .Where(hs => hs.SpecialtyId == specialtyId)
                .Select(hs => hs.HospitalAsset)
                .Distinct(); 
        }



    }
    }

