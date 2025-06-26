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
        Task<IEnumerable<Doctor>> GetDoctorsByHospitalSpecialtyAsync(int hospitalSpecialtyId); // For DoctorTableDto
        public Task<Doctor> GetDoctorIdAsync(int id);

        Task<Doctor> GetDoctorByIdWithHospitalSpecialtyAndSpecialtyAsync(string doctorId);

        // For Admin Dashboard: Get all doctors with their full navigation for the table view
        Task<IEnumerable<Doctor>> GetAllDoctorsWithHospitalSpecialtyAndSpecialtyAsync(); 
    }
}
