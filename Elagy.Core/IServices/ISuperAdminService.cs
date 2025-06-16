using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using Elagy.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface ISuperAdminService
    {
        Task<SuperAdminDto> GetSuperAdminProfileAsync(string superAdminId);

        Task<AuthResultDto> ActivateUserAccountAsync(string userId);
        Task<AuthResultDto> DeactivateUserAccountAsync(string userId);
        Task<AuthResultDto> ApproveServiceProviderAccountAsync(string providerId);
        Task<AuthResultDto> RejectServiceProviderAccountAsync(string providerId, string? rejectionReason); 


        Task<PagedResponseDto<PatientDto>> GetPatientsForAdminDashboardAsync(PaginationParameters paginationParameters); 
        Task< PagedResponseDto<HotelProviderProfileDto>> GetHotelProvidersForAdminDashboardAsync(PaginationParameters paginationParameters); 
        Task<PagedResponseDto<HospitalProviderProfileDto>> GetHospitalProvidersForAdminDashboardAsync(PaginationParameters paginationParameters); 
        Task<PagedResponseDto<CarRentalProviderProfileDto>> GetCarRentalProvidersForAdminDashboardAsync(PaginationParameters paginationParameters); 



        //----------------- i will remove them soon ----------------------
        Task<PatientDto> GetPatientDetailsForAdminAsync(string patientId);
        Task<HotelProviderProfileDto> GetHotelProviderDetailsForAdminAsync(string providerId);
        Task<HospitalProviderProfileDto> GetHospitalProviderDetailsForAdminAsync(string providerId);
        Task<CarRentalProviderProfileDto> GetCarRentalProviderDetailsForAdminAsync(string providerId);

        Task<AuthResultDto> AdminChangeUserEmailAsync(string userId, string newEmail); // newEmail is not nullable
        Task<AuthResultDto> AdminResetUserPasswordAsync(string userId);
     }
}