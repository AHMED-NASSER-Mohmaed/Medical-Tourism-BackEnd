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
        //Task<IQueryable<HospitalAsset>> GetFilteredHospitalsQueryAsync(HospitalFilterDto filter); // Return IQueryable for service to project
        //Task<HospitalAsset> GetHospitalWithSpecialtiesAsync(string hospitalId); // For HospitalWebsiteDto
        public Task<IEnumerable<HospitalSpecialty>> GetHospitalSpecialtiesInHospitalAsync(string hospitalId); // Get join entities for HospitalSpecialtyDto
   
       public Task<IQueryable<HospitalAsset>> GetHospitalsBySpecialtyQueryAsync(int specialtyId); 
        //Task<IQueryable<HospitalAsset>> GetHospitalsBySpecialtyQueryAsync(int specialtyId); // Return IQueryable for service to project
    }
}
