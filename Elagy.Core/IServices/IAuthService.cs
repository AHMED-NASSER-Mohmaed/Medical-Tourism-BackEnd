using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IAuthService
    {
        // User registration (distinct routes, distinct UIs)
        Task<AuthResultDto> RegisterPatientAsync(PatientRegistrationRequestDto model);
        Task<AuthResultDto> RegisterHotelProviderAsync(HotelAssetRegistrationRequestDto model, List<IFormFile> files);
        Task<AuthResultDto> RegisterHospitalProviderAsync(HospitalAssetRegistrationRequestDto model, List<IFormFile> files);
        Task<AuthResultDto> RegisterCarRentalProviderAsync(CarRentalAssetRegistrationRequestDto model, List<IFormFile> files);
        Task<AuthResultDto> RegisterSuperAdminAsync(SuperAdminRegistrationRequestDto model); // Admin onboarding

        // Core Identity operations
        Task<AuthResultDto> LoginAsync(LoginRequestDto model);
        Task<AuthResultDto> ConfirmEmailAsync(string userId, string token);
        Task<AuthResultDto> ForgotPasswordAsync(string email);
        Task<AuthResultDto> ResetPasswordAsync(string userId,ResetPasswordRequestDto model); // User initiated password reset
        Task<AuthResultDto> ChangePasswordAsync(string userId, ChangePasswordRequestDto model); // User initiated password change
        Task<AuthResultDto> ChangeEmailAsync(string userId, ChangeEmailRequestDto model); // User initiated email change

        Task<AuthResultDto> ConfirmNewEmailAsync(string userId, string newEmail, string token);

    }
}