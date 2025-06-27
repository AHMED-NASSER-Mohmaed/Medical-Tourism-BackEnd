using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IHospitalRepository : IGenericRepository<HospitalAsset>
    {
        
        public Task<IEnumerable<HospitalSpecialty>> GetHospitalSpecialtiesInHospitalAsync(string hospitalId);
        public Task<IQueryable<HospitalAsset>> GetHospitalsBySpecialtyQueryAsync(int specialtyId);


    }
}
