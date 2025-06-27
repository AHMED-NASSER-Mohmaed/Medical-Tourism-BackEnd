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

        Task<PagedResponseDto<DoctorProfileDto>> GetAllDoctorsForAdminDashboardAsync(string hospitalId); // For table display

        //for external web site 
        //dispaly doctors by clinic and hospital for patient 
        Task<PagedResponseDto<DoctorProfileDto>> GetAllDoctorsPerHospitalSpecility(string hospitalspecialityyId);  



        //hopital id is token from [jwt token] 
        Task<DoctorProfileDto> CreateDoctorAsync(DoctorCreateDto createDto,string hospitalId);
        
        //hopital id is token from [jwt token] 
        Task<DoctorProfileDto> UpdateDoctorAsync(DoctorUpdateDto updateDto, string hospitalId);

        //hopital id is token from [jwt token]
        // doctor id from params
        Task<DoctorProfileDto> DeleteDoctorAsync(string id , string hospitalId);


        // Website View (when clicking on a specialty within a hospital)
        //Task<IEnumerable<DoctorWebsiteDto>> GetDoctorsForWebsiteByHospitalSpecialtyAsync(string hospitalId, int specialtyId);
    }
}
