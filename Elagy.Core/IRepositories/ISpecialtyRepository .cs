using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface ISpecialtyRepository : IGenericRepository<Specialty>
    {
        Task<Specialty> GetSpecialtyByIdWithHospitalsAsync(int id);
        public Task<Specialty> GetSpecialtyIdAsync(int id);
        Task<bool> ExistsByNameAsync(string name); // <--- It is declared HERE
        void SoftDelete(Specialty entity);
    }
}
