using AutoMapper;
using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using Elagy.Core.Entities;
using ServiceProvider = Elagy.Core.Entities.ServiceProvider; // Ensure this is the correct namespace for ServiceProvider
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Required for .Include() and .ToListAsync()
using Microsoft.Extensions.Logging; // Add this line if it's missing
using Elagy.Core.Helpers; // Required for Guid and DateTime.UtcNow

namespace Elagy.BL.Services
{
    public class SuperAdminService : ISuperAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SuperAdminService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        // Inject the specific services needed for getting detailed profiles and admin-added functionality
        private readonly IPatientService _patientService;
        private readonly IHotelProviderService _hotelProviderService;
        private readonly IHospitalProviderService _hospitalProviderService;
        private readonly ICarRentalProviderService _carRentalProviderService;

        public SuperAdminService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<SuperAdminService> logger,
            UserManager<User> userManager,
            IEmailService emailService,
            IPatientService patientService,
            IHotelProviderService hotelProviderService,
            IHospitalProviderService hospitalProviderService,
            ICarRentalProviderService carRentalProviderService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _emailService = emailService;

            _patientService = patientService;
            _hotelProviderService = hotelProviderService;
            _hospitalProviderService = hospitalProviderService;
            _carRentalProviderService = carRentalProviderService;
        }

        public async Task<SuperAdminDto> GetSuperAdminProfileAsync(string superAdminId)
        {
            var superAdmin = await _unitOfWork.SuperAdmins.GetByIdAsync(superAdminId);
            if (superAdmin == null)
            {
                _logger.LogWarning($"Super Admin with ID {superAdminId} not found.");
                return null;
            }
            return _mapper.Map<SuperAdminDto>(superAdmin);
        }

        // --- General User/Account Management ---

        public async Task<AuthResultDto> ActivateUserAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "User not found." } };
            }
            if (user.Status == UserStatus.Active)
            {
                return new AuthResultDto { Success = true, Message = "User is already active." };
            }

            user.Status = UserStatus.Active;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {userId} activated successfully.");
                return new AuthResultDto { Success = true, Message = "User account activated." };
            }
            _logger.LogError($"Failed to activate user {userId}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
        }

        public async Task<AuthResultDto> DeactivateUserAccountAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "User not found." } };
            }
            if (user.Status == UserStatus.Deactivated)
            {
                return new AuthResultDto { Success = true, Message = "User is already deactivated." };
            }

            user.Status = UserStatus.Deactivated;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {userId} deactivated successfully.");
                return new AuthResultDto { Success = true, Message = "User account deactivated." };
            }
            _logger.LogError($"Failed to deactivate user {userId}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
        }

        public async Task<AuthResultDto> ApproveServiceProviderAccountAsync(string providerId)
        {
            var provider = await _unitOfWork.ServiceProviders.GetByIdAsync(providerId);
            if (provider == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Service Provider not found." } };
            }
            if (provider.Status == UserStatus.Active)
            {
                return new AuthResultDto { Success = true, Message = "Service Provider is already approved and active." };
            }

            provider.Status = UserStatus.Active;
            var userUpdateResult = await _userManager.UpdateAsync(provider);

            if (!userUpdateResult.Succeeded)
            {
                _logger.LogError($"Failed to approve SP user status for {providerId}: {string.Join(", ", userUpdateResult.Errors.Select(e => e.Description))}");
                return new AuthResultDto { Success = false, Errors = userUpdateResult.Errors.Select(e => e.Description) };
            }

            // Also set ServiceAsset's verification status to Verified
            var asset = await _unitOfWork.ServiceAssets.GetByIdAsync(providerId);
            if (asset != null)
            {
                asset.VerificationStatus = VerificationStatus.Verified;
                _unitOfWork.ServiceAssets.Update(asset);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Service Asset {asset.Id} verification status set to Verified for approved provider {providerId}.");
            }
            else
            {
                _logger.LogWarning($"Service Asset not found for approved provider {providerId}. Asset verification status not updated.");
            }

            _logger.LogInformation($"Service Provider {providerId} approved successfully.");
            return new AuthResultDto { Success = true, Message = "Service Provider account approved." };
        }

        public async Task<AuthResultDto> RejectServiceProviderAccountAsync(string providerId, string rejectionReason)
        {
            var provider = await _unitOfWork.ServiceProviders.GetByIdAsync(providerId);
            if (provider == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Service Provider not found." } };
            }

            // Deactivate user account
            provider.Status = UserStatus.Deactivated;
            var userUpdateResult = await _userManager.UpdateAsync(provider);

            if (!userUpdateResult.Succeeded)
            {
                _logger.LogError($"Failed to reject SP user status for {providerId}: {string.Join(", ", userUpdateResult.Errors.Select(e => e.Description))}");
                return new AuthResultDto { Success = false, Errors = userUpdateResult.Errors.Select(e => e.Description) };
            }

            // Also set ServiceAsset's verification status to Rejected and add notes
            var asset = await _unitOfWork.ServiceAssets.GetByIdAsync(providerId);
            if (asset != null)
            {
                asset.VerificationStatus = VerificationStatus.Rejected;
                asset.VerificationNotes = rejectionReason;
                _unitOfWork.ServiceAssets.Update(asset);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Service Asset {asset.Id} verification status set to Rejected for provider {providerId}. Reason: {rejectionReason}");
            }
            else
            {
                _logger.LogWarning($"Service Asset not found for rejected provider {providerId}. Asset verification status not updated.");
            }

            _logger.LogInformation($"Service Provider {providerId} rejected with reason: {rejectionReason}");
            return new AuthResultDto { Success = true, Message = "Service Provider account rejected." };
        }

        // --- Filtering/Listing for Dashboard ---

        private IQueryable<User> ApplyUserFilters(IQueryable<User> query, string searchQuery, UserStatus? status)
        {
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string lowerSearchQuery = searchQuery.ToLower();
                query = query.Where(u => u.Email.ToLower().Contains(lowerSearchQuery) ||
                                         u.FirstName.ToLower().Contains(lowerSearchQuery) ||
                                         u.LastName.ToLower().Contains(lowerSearchQuery) ||
                                         u.Id.Contains(lowerSearchQuery)); // Search by ID
            }
            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }
            return query;
        }

        private IQueryable<ServiceProvider> ApplyServiceProviderFilters(IQueryable<ServiceProvider> query, string searchQuery, UserStatus? userStatus, VerificationStatus? assetStatus)
        {
            // Ensure ServiceAsset is included for filtering on its properties
            query = query.Include(sp => sp.ServiceAsset);

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string lowerSearchQuery = searchQuery.ToLower();
                query = query.Where(sp => sp.Email.ToLower().Contains(lowerSearchQuery) ||
                                         sp.FirstName.ToLower().Contains(lowerSearchQuery) ||
                                         sp.LastName.ToLower().Contains(lowerSearchQuery) ||
                                         sp.Id.Contains(lowerSearchQuery) ||
                                         (sp.ServiceAsset != null && (sp.ServiceAsset.AssetName.ToLower().Contains(lowerSearchQuery) ||
                                                                       sp.ServiceAsset.Description.ToLower().Contains(lowerSearchQuery) ||
                                                                       sp.ServiceAsset.LocationDescription.ToLower().Contains(lowerSearchQuery))));
            }
            if (userStatus.HasValue)
            {
                query = query.Where(sp => sp.Status == userStatus.Value);
            }
            if (assetStatus.HasValue)
            {
                query = query.Where(sp => sp.ServiceAsset != null && sp.ServiceAsset.VerificationStatus == assetStatus.Value);
            }
            return query;
        }

        public async Task<IEnumerable<PatientDto>> GetPatientsForAdminDashboardAsync(int page, int limit, string searchQuery = null, UserStatus? status = null)
        {
            var query = _unitOfWork.Patients.AsQueryable().OfType<Patient>(); // Use AsQueryable()
            query = ApplyUserFilters(query.Cast<User>(), searchQuery, status).OfType<Patient>(); // Apply filters, cast back to Patient
            return _mapper.Map<IEnumerable<PatientDto>>(await query.Skip((page - 1) * limit).Take(limit).ToListAsync());
        }

        public async Task<IEnumerable<HotelProviderProfileDto>> GetHotelProvidersForAdminDashboardAsync(int page, int limit, string searchQuery = null, UserStatus? userStatus = null, VerificationStatus? assetStatus = null)
        {
            var query = _unitOfWork.ServiceProviders.AsQueryable() // Use AsQueryable()
                .OfType<ServiceProvider>() // Filter to only ServiceProvider types
                .Where(sp => sp.ServiceAsset != null && sp.ServiceAsset.AssetType == AssetType.Hotel); // Filter for Hotel assets

            query = ApplyServiceProviderFilters(query, searchQuery, userStatus, assetStatus);
            return _mapper.Map<IEnumerable<HotelProviderProfileDto>>(await query.Skip((page - 1) * limit).Take(limit).ToListAsync());
        }

        public async Task<IEnumerable<HospitalProviderProfileDto>> GetHospitalProvidersForAdminDashboardAsync(int page, int limit, string searchQuery = null, UserStatus? userStatus = null, VerificationStatus? assetStatus = null)
        {
            var query = _unitOfWork.ServiceProviders.AsQueryable()
                .OfType<ServiceProvider>()
                .Where(sp => sp.ServiceAsset != null && sp.ServiceAsset.AssetType == AssetType.Hospital);

            query = ApplyServiceProviderFilters(query, searchQuery, userStatus, assetStatus);
            return _mapper.Map<IEnumerable<HospitalProviderProfileDto>>(await query.Skip((page - 1) * limit).Take(limit).ToListAsync());
        }

        public async Task<IEnumerable<CarRentalProviderProfileDto>> GetCarRentalProvidersForAdminDashboardAsync(int page, int limit, string searchQuery = null, UserStatus? userStatus = null, VerificationStatus? assetStatus = null)
        {
            var query = _unitOfWork.ServiceProviders.AsQueryable()
                .OfType<ServiceProvider>()
                .Where(sp => sp.ServiceAsset != null && sp.ServiceAsset.AssetType == AssetType.CarRental);

            query = ApplyServiceProviderFilters(query, searchQuery, userStatus, assetStatus);
            return _mapper.Map<IEnumerable<CarRentalProviderProfileDto>>(await query.Skip((page - 1) * limit).Take(limit).ToListAsync());
        }


        // --- Get Full Details for Admin Modal (Delegating to specific services) ---
        public async Task<PatientDto> GetPatientDetailsForAdminAsync(string patientId)
        {
            return await _patientService.GetPatientProfileAsync(patientId);
        }

        public async Task<HotelProviderProfileDto> GetHotelProviderDetailsForAdminAsync(string providerId)
        {
            return await _hotelProviderService.GetHotelProviderProfileAsync(providerId);
        }

        public async Task<HospitalProviderProfileDto> GetHospitalProviderDetailsForAdminAsync(string providerId)
        {
            return await _hospitalProviderService.GetHospitalProviderProfileAsync(providerId);
        }

        public async Task<CarRentalProviderProfileDto> GetCarRentalProviderDetailsForAdminAsync(string providerId)
        {
            return await _carRentalProviderService.GetCarRentalProviderProfileAsync(providerId);
        }


        // --- Admin-initiated Email/Password Changes and Asset Verification ---
        public async Task<AuthResultDto> AdminChangeUserEmailAsync(string userId, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "User not found." } };
            }

            // Check if new email is already taken by another user
            var existingUserWithNewEmail = await _userManager.FindByEmailAsync(newEmail);
            if (existingUserWithNewEmail != null && existingUserWithNewEmail.Id != user.Id)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "New email address is already taken by another user." } };
            }

            // Generate a token for email change. Admin initiated, so no current password needed.
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            // IMPORTANT: Replace with your actual frontend URL
            var confirmationLink = $"YOUR_FRONTEND_APP_URL/confirm-new-email?userId={user.Id}&newEmail={Uri.EscapeDataString(newEmail)}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(newEmail, "Confirm Your New Email Address (Admin Initiated)", $"Your email address was changed by an administrator. Please confirm your new email by clicking this link: <a href='{confirmationLink}'>link</a>");

            // Optionally, mark user's EmailConfirmed to false until they confirm new email.
            // This is a security measure to prevent login with unconfirmed new email.
            user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation($"Admin initiated email change for user {user.Email} to {newEmail}. Confirmation email sent to new address.");
            return new AuthResultDto { Success = true, Message = "Email change initiated. Confirmation email sent to the new address. User needs to confirm the new email to activate it." };
        }

        public async Task<AuthResultDto> AdminResetUserPasswordAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "User not found." } };
            }

            // Generate a password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // Generate a *new, random* password (recommended for admin reset)
            string newRandomPassword = GenerateRandomPassword(); // Implement this helper method

            // Reset password using the token and the generated password
            var resetResult = await _userManager.ResetPasswordAsync(user, token, newRandomPassword);

            if (resetResult.Succeeded)
            {
                // Send the new password to the user's *confirmed* email
                await _emailService.SendEmailAsync(user.Email, "Your Password Has Been Reset",
                    $"Your account password has been reset by an administrator. Your new temporary password is: <strong>{newRandomPassword}</strong>. Please log in and change your password immediately.");
                _logger.LogInformation($"Admin reset password for user {user.Email}. New password sent to email.");
                return new AuthResultDto { Success = true, Message = "A new password has been generated and sent to the user's registered email address." };
            }

            _logger.LogError($"Admin password reset failed for user {user.Email}: {string.Join(", ", resetResult.Errors.Select(e => e.Description))}");
            return new AuthResultDto { Success = false, Errors = resetResult.Errors.Select(e => e.Description) };
        }

        public async Task<AuthResultDto> AdminSetAssetVerificationStatusAsync(string assetId, VerificationStatus status, string notes = null)
        {
            var asset = await _unitOfWork.ServiceAssets.GetByIdAsync(assetId);
            if (asset == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Service Asset not found." } };
            }

            asset.VerificationStatus = status;
            asset.VerificationNotes = notes;
            _unitOfWork.ServiceAssets.Update(asset);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation($"Asset {assetId} verification status set to {status} by admin.");
            return new AuthResultDto { Success = true, Message = $"Asset verification status updated to {status}." };
        }

        // Helper method to generate a random password
        private string GenerateRandomPassword()
        {
            // IMPORTANT: In a production environment, use a more robust and secure password generation library
            // that considers UserManager's password options (e.g., required length, symbols, digits).
            // This is a simple example for compilation.

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }
    }
}