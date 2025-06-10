using Elagy.Core.DTOs.Auth;
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
        Task<AuthResultDto> RejectServiceProviderAccountAsync(string providerId, string? rejectionReason); // Corrected

        Task<IEnumerable<PatientDto>> GetPatientsForAdminDashboardAsync(
            int page, int limit, string? searchQuery = null, UserStatus? status = null); // Corrected

        Task<IEnumerable<HotelProviderProfileDto>> GetHotelProvidersForAdminDashboardAsync(
            int page, int limit, string? searchQuery = null, UserStatus? userStatus = null, VerificationStatus? assetStatus = null); // Corrected

        Task<IEnumerable<HospitalProviderProfileDto>> GetHospitalProvidersForAdminDashboardAsync(
            int page, int limit, string? searchQuery = null, UserStatus? userStatus = null, VerificationStatus? assetStatus = null); // Corrected

        Task<IEnumerable<CarRentalProviderProfileDto>> GetCarRentalProvidersForAdminDashboardAsync(
            int page, int limit, string? searchQuery = null, UserStatus? userStatus = null, VerificationStatus? assetStatus = null); // Corrected

        Task<PatientDto> GetPatientDetailsForAdminAsync(string patientId);
        Task<HotelProviderProfileDto> GetHotelProviderDetailsForAdminAsync(string providerId);
        Task<HospitalProviderProfileDto> GetHospitalProviderDetailsForAdminAsync(string providerId);
        Task<CarRentalProviderProfileDto> GetCarRentalProviderDetailsForAdminAsync(string providerId);

        Task<AuthResultDto> AdminChangeUserEmailAsync(string userId, string newEmail); // newEmail is not nullable
        Task<AuthResultDto> AdminResetUserPasswordAsync(string userId);
        Task<AuthResultDto> AdminSetAssetVerificationStatusAsync(string assetId, VerificationStatus status, string? notes = null); // Corrected
    }
}