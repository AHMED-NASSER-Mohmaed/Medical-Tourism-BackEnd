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

        public async Task<HospitalSpecialty?> GetByHospitalAndSpecialtyIdAsync(string hospitalId, int specialtyId)
        {
            return await _dbSet
                 .FirstOrDefaultAsync(hs => hs.HospitalAssetId == hospitalId && hs.SpecialtyId == specialtyId);
        }

        public async Task<HospitalSpecialty?> GetByIdWithDetailsAsync(int hospitalspecialtyid)
        {
            return await _dbSet // _dbSet is from GenericRepository<HospitalSpecialty, int>
               .Where(hs => hs.Id == hospitalspecialtyid)
               .Include(hs => hs.Specialty) // Include the Specialty navigation property
               .Include(hs => hs.HospitalAsset) // Include the HospitalAsset navigation property
               .FirstOrDefaultAsync();
        }


    }
}
