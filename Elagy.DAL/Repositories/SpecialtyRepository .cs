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

        public async Task<Specialty?> GetSpecialtyByIdWithHospitalsAsync(int id)
        {
            return await _dbSet.Where(s => s.Id == id)
                                 .Include(s => s.HospitalSpecialties)
                                     .ThenInclude(hs => hs.HospitalAsset)
                                 .FirstOrDefaultAsync();
        }
        public async Task<Specialty?> GetSpecialtyIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _dbSet.AnyAsync(s => s.Name == name && !s.IsDeleted); // <--- It is implemented HERE
        }

        public void SoftDelete(Specialty entity)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow; // Sets the deletion timestamp
            _dbSet.Update(entity); // Marks the entity as modified so EF Core saves the changes
        }
    }
}
