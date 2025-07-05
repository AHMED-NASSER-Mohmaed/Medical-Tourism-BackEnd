using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IDoctorRepository : IGenericRepository<Doctor>
    {
        //for hospital admin
        Task<IEnumerable<Doctor>> GetDoctorsByHospitalIdAsync(string hospitalId);
        // for website show 
        Task<IEnumerable<Doctor>> GetDoctorsByHospitalSpecialtyIdAsync(int hospitalSpecialtyId);
        //show doctor details
        Task<Doctor?> GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(string doctorId);

        Task<IEnumerable<Doctor>> GetDoctorsBySpecialtyIdAsync(int specialtyId);
    }
}
