using Elagy.Core.DTOs.Doctor;
using Elagy.Core.DTOs.Pagination;
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

        // MODIFIED: Added PaginationParameters
        Task<PagedResponseDto<DoctorProfileDto>> GetAllDoctorsForAdminDashboardAsync(string hospitalId, PaginationParameters paginationParameters);

        // MODIFIED: Added PaginationParameters, renamed parameter for clarity
        Task<PagedResponseDto<DoctorProfileDto>> GetAllDoctorsPerHospitalSpecialty(int hospitalSpecialtyId, PaginationParameters paginationParameters);

        // hospital id is token from [jwt token]
        Task<DoctorProfileDto> CreateDoctorAsync(DoctorCreateDto createDto, string hospitalId);

        // MODIFIED: Added doctorId as a separate parameter, hospitalId from token
        Task<DoctorProfileDto> UpdateDoctorAsync(string doctorId, DoctorUpdateDto updateDto, string hospitalId);

        // hospital id is token from [jwt token], doctor id from params
        Task<DoctorProfileDto> DeleteDoctorAsync(string doctorId, string hospitalId);
        Task<DoctorProfileDto?> GetDoctorByIdAsync(string doctorId);


    }
}
