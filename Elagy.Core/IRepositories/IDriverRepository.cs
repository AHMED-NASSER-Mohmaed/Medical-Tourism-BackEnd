using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IDriverRepository : IGenericRepository<Driver>
    {

        /// Gets all drivers affiliated with a specific car rental asset.
        Task<IEnumerable<Driver>> GetDriversByCarRentalAssetIdAsync(string carRentalAssetId, Status? driverStatus = null);

        /// Gets a single driver by their ID, including all related details (CarRentalAsset, CarDrivers).
        Task<Driver?> GetDriverByIdWithDetailsAsync(string driverId);

        /// Gets the current active assignment for a specific driver.
        Task<CarDriver?> GetCurrentCarAssignmentForDriverAsync(string driverId);
    }
}
