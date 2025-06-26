using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IHospitalSpecialtyRepository : IGenericRepository<HospitalSpecialty>
    {
        Task<HospitalSpecialty> GetByIdAsync(int id);
        Task<IEnumerable<HospitalSpecialty>> GetActiveHospitalSpecialtiesByHospitalIdAsync(string hospitalId);
    }
}
