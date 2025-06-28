using Elagy.Core.DTOs.Specialty;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface ISpecialtyRepository : IGenericRepository<Specialty>
    {
    
       Task<Specialty> GetSpecialtyIdAsync(int id);
        Task<IEnumerable<Specialty>> GetSpecialtiesByHospitalIdAsync(string hospitalId, bool isActive = true);
        Task<IEnumerable<Specialty>> GetAllSpecialtiesAsync(bool isActive = true);
        Task<IEnumerable<Specialty>> GetUnlinkedSpecialtiesForHospitalAsync(string hospitalId);
      


    }
}
