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

        Task<IEnumerable<Car>> GetCarsByCarRentalAssetIdAsync(string carRentalAssetId);

        Task<Car?> GetCarByIdWithDetailsAsync(int carId);
    }
}
