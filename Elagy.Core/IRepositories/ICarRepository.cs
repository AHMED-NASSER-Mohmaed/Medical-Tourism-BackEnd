using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface ICarRepository : IGenericRepository<Car>
    {
        /// Gets all cars for a specific car rental asset.
        Task<IEnumerable<Car>> GetCarsByCarRentalAssetIdAsync(string carRentalAssetId, bool? isAvailable = null, Enums.CarStatus? status = null);

        /// Gets a single car by its ID, including all related details (CarRentalAsset, CarImages, CarDrivers).
        Task<Car?> GetCarByIdWithDetailsAsync(int carId);
    }
}
