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
    public class DriverRepository : GenericRepository<Driver>, IDriverRepository
    {
   

        public DriverRepository(ApplicationDbContext context) : base(context)
        {
          
        }

        // Helper method to include common navigation properties for Driver details
        private IQueryable<Driver> GetDriversWithAllDetails()
        {
            return _dbSet
                .Include(d => d.CarRentalAsset) 
                .Include(d => d.CarDrivers) 
                    .ThenInclude(cd => cd.Car); 
        } 

        public async Task<IEnumerable<Driver>> GetDriversByCarRentalAssetIdAsync(string carRentalAssetId, Status? driverStatus = null)
        {
            var query = GetDriversWithAllDetails()
                .Where(d => d.CarRentalAssetId == carRentalAssetId);

            if (driverStatus.HasValue)
            {
                query = query.Where(d => d.Status == driverStatus.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<Driver?> GetDriverByIdWithDetailsAsync(string driverId)
        {
            return await GetDriversWithAllDetails()
                .FirstOrDefaultAsync(d => d.Id == driverId);
        }

        public async Task<CarDriver?> GetCurrentCarAssignmentForDriverAsync(string driverId)
        {
            return await _context.CarDrivers // Access DbSet directly for CarDriver
                .Include(cd => cd.Car) // Include the Car details
                .Where(cd => cd.DriverId == driverId && cd.IsAssignedCurrent == true && cd.ReleaseDate == null)
                .FirstOrDefaultAsync();
        }
    }
}
