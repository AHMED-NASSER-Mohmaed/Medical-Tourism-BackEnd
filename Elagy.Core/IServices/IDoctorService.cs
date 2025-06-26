using Elagy.Core.DTOs.Doctor;
using Elagy.Core.DTOs.Specialty;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IDoctorService
    {
        // Admin Dashboard CRUD
        Task<IEnumerable<DoctorTableDto>> GetAllDoctorsForAdminDashboardAsync(string hospitalId); // For table display
        Task<DoctorTableDto> GetDoctorByIdForAdminAsync(string id, string hospitalId); // For view details modal
        Task<DoctorTableDto> CreateDoctorAsync(DoctorCreateDto createDto, string hospitalId);
        Task<bool> UpdateDoctorAsync(DoctorUpdateDto updateDto, string hospitalId);
        Task<bool> DeleteDoctorAsync(string id, string hospitalId);

        // Admin Helper: Get available HospitalSpecialties for doctor assignment dropdown
        Task<IEnumerable<HospitalSpecialtyDto>> GetAvailableHospitalSpecialtiesForAssignmentAsync(string hospitalId);

        // Website View (when clicking on a specialty within a hospital)
        //Task<IEnumerable<DoctorWebsiteDto>> GetDoctorsForWebsiteByHospitalSpecialtyAsync(string hospitalId, int specialtyId);
    }
}
