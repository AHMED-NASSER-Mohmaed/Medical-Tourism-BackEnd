using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface ICarDriverRepository : IGenericRepository<CarDriver>
    {
        Task<IEnumerable<CarDriver>> GetAssignmentsByCarIdAsync(int carId, bool? isCurrent = null);
        Task<IEnumerable<CarDriver>> GetAssignmentsByDriverIdAsync(string driverId, bool? isCurrent = null);
        Task<CarDriver?> GetCarDriverByIdWithDetailsAsync(int carDriverId);
    }
}
