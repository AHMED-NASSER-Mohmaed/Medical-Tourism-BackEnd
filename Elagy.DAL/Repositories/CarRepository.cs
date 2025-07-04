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
    public class CarRepository : GenericRepository<Car>, ICarRepository
    {
      

        public CarRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        // Helper method to include common navigation properties for Car details
        private IQueryable<Car> GetCarsWithAllDetails()
        {
            return _dbSet
                .Include(c => c.CarRentalAsset)
                .Include(c => c.CarImages)
                .Include(c => c.CarDrivers)
                    .ThenInclude(cd => cd.Driver);
        }

        public async Task<IEnumerable<Car>> GetCarsByCarRentalAssetIdAsync(string carRentalAssetId)
        {
            var query = GetCarsWithAllDetails()
                .Where(c => c.CarRentalAssetId == carRentalAssetId);

            return await query.ToListAsync();
        }

        public async Task<Car?> GetCarByIdWithDetailsAsync(int carId)
        {
            return await GetCarsWithAllDetails()
                .FirstOrDefaultAsync(c => c.Id == carId);
        }
    }
}
