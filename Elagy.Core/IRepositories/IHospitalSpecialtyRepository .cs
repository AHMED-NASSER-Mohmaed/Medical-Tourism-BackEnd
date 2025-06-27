using Elagy.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IHospitalSpecialtyRepository : IGenericRepository<HospitalSpecialty>
    {
        Task<HospitalSpecialty?> GetByIdWithDetailsAsync(int hospitalspecialtyid);
        Task<HospitalSpecialty?> GetByHospitalAndSpecialtyIdAsync(string hospitalId, int specialtyId);

    }
}
