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
    public class CarDriverRepository : GenericRepository<CarDriver>, ICarDriverRepository
    {
        public CarDriverRepository(ApplicationDbContext context) : base(context)
        {
    
        }

        // Helper to include necessary navigation properties for CarDriver details
        private IQueryable<CarDriver> GetCarDriverDetails()
        {
            return _dbSet
                .Include(cd => cd.Car)
                .Include(cd => cd.Driver);
        }

        public async Task<IEnumerable<CarDriver>> GetAssignmentsByCarIdAsync(int carId, bool? isCurrent = null)
        {
            var query = GetCarDriverDetails()
                .Where(cd => cd.CarId == carId);

            if (isCurrent.HasValue)
            {
                query = query.Where(cd => cd.IsAssignedCurrent == isCurrent.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<CarDriver>> GetAssignmentsByDriverIdAsync(string driverId, bool? isCurrent = null)
        {
            var query = GetCarDriverDetails()
                .Where(cd => cd.DriverId == driverId);

            if (isCurrent.HasValue)
            {
                query = query.Where(cd => cd.IsAssignedCurrent == isCurrent.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<CarDriver?> GetCarDriverByIdWithDetailsAsync(int carDriverId)
        {
            return await GetCarDriverDetails()
                .FirstOrDefaultAsync(cd => cd.Id == carDriverId);
        }
    }
}
