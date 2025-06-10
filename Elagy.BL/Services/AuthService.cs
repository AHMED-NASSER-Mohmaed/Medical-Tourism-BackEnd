using AutoMapper;
using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.Entities;
using ServiceProvider= Elagy.Core.Entities.ServiceProvider;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IEmailService = Elagy.Core.Helpers.IEmailService;
using IJwtTokenGenerator = Elagy.Core.Helpers.IJwtTokenGenerator;
using Microsoft.Extensions.Logging; // Add this line if it's missing

namespace Elagy.BL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEmailService emailService,
            IJwtTokenGenerator jwtTokenGenerator,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _logger = logger;
        }

        // --- Registration Methods ---

        private async Task<AuthResultDto> RegisterUserBaseAsync<TUser, TDto>(TDto model, UserType userType, Func<TUser> createUser)
            where TUser : User
            where TDto : BaseRegistrationRequestDto
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Email already registered." } };
            }

            var user = createUser(); // Create specific user type (Patient, ServiceProvider, SuperAdmin)
            _mapper.Map(model, user); // Map common properties
            user.UserType = userType;
            user.Status = UserStatus.EmailUnconfirmed; // Initial status

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                _logger.LogError($"User creation failed for {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }

            // Send email confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"YOUR_FRONTEND_APP_URL/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}"; // IMPORTANT: Use actual frontend URL
            await _emailService.SendEmailAsync(user.Email, "Confirm Your Email", $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>");

            _logger.LogInformation($"User {user.Email} registered successfully. Confirmation email sent.");
            return new AuthResultDto { Success = true, Message = "Registration successful. Please confirm your email." };
        }

        public async Task<AuthResultDto> RegisterPatientAsync(PatientRegistrationRequestDto model)
        {
            return await RegisterUserBaseAsync<Patient, PatientRegistrationRequestDto>(model, UserType.Patient, () => new Patient());
        }

        public async Task<AuthResultDto> RegisterHotelProviderAsync(HotelProviderRegistrationRequestDto model)
        {
            return await RegisterServiceProviderAsync<HotelAsset, HotelProviderRegistrationRequestDto>(model, AssetType.Hotel, () => new HotelAsset());
        }

        public async Task<AuthResultDto> RegisterHospitalProviderAsync(HospitalProviderRegistrationRequestDto model)
        {
            return await RegisterServiceProviderAsync<HospitalAsset, HospitalProviderRegistrationRequestDto>(model, AssetType.Hospital, () => new HospitalAsset());
        }

        public async Task<AuthResultDto> RegisterCarRentalProviderAsync(CarRentalProviderRegistrationRequestDto model)
        {
            return await RegisterServiceProviderAsync<CarRentalAsset, CarRentalProviderRegistrationRequestDto>(model, AssetType.CarRental, () => new CarRentalAsset());
        }

        private async Task<AuthResultDto> RegisterServiceProviderAsync<TAsset, TDto>(TDto model, AssetType assetType, Func<TAsset> createAsset)
            where TAsset : ServiceAsset
            where TDto : BaseServiceProviderRegistrationRequestDto
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Email already registered." } };
            }

            var serviceProvider = _mapper.Map<ServiceProvider>(model);
            serviceProvider.UserType = UserType.ServiceProvider;
            serviceProvider.Status = UserStatus.EmailUnconfirmed; // Initial status

            var result = await _userManager.CreateAsync(serviceProvider, model.Password);

            if (!result.Succeeded)
            {
                _logger.LogError($"Service Provider creation failed for {serviceProvider.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }

            // Create the associated ServiceAsset
            var serviceAsset = createAsset();
            _mapper.Map(model, serviceAsset); // Map common asset properties from DTO
            serviceAsset.Id = serviceProvider.Id; // Set AssetId to match ServiceProvider's Id (shared PK)
            serviceAsset.ServiceProvider = serviceProvider; // Link to the provider
            serviceAsset.AssetType = assetType;
            serviceAsset.AcquisitionDate = DateTime.UtcNow; // Set creation date
            serviceAsset.VerificationStatus = VerificationStatus.Pending; // Initial verification status

            try
            {
                await _unitOfWork.ServiceAssets.AddAsync(serviceAsset);
                await _unitOfWork.CompleteAsync(); // Save the asset and link
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, $"Failed to create ServiceAsset for {serviceProvider.Email}. Rolling back user creation.");
                await _userManager.DeleteAsync(serviceProvider); // Rollback user creation
                return new AuthResultDto { Success = false, Errors = new[] { "Failed to create associated asset. Please try again." } };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error creating ServiceAsset for {serviceProvider.Email}. Rolling back user creation.");
                await _userManager.DeleteAsync(serviceProvider); // Rollback user creation
                return new AuthResultDto { Success = false, Errors = new[] { "An unexpected error occurred during asset creation." } };
            }

            // Send email confirmation
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(serviceProvider);
            var confirmationLink = $"YOUR_FRONTEND_APP_URL/confirm-email?userId={serviceProvider.Id}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendEmailAsync(serviceProvider.Email, "Confirm Your Email", $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>");

            _logger.LogInformation($"Service Provider {serviceProvider.Email} registered successfully with asset. Confirmation email sent. Awaiting admin approval.");
            return new AuthResultDto { Success = true, Message = "Registration successful. Please confirm your email. Your account will be active after admin approval." };
        }

        public async Task<AuthResultDto> RegisterSuperAdminAsync(SuperAdminRegistrationRequestDto model)
        {
            var result = await RegisterUserBaseAsync<SuperAdmin, SuperAdminRegistrationRequestDto>(model, UserType.SuperAdmin, () => new SuperAdmin());
            if (result.Success)
            {
                // Optionally assign a "SuperAdmin" role here if you use roles explicitly
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    await _userManager.AddToRoleAsync(user, "SuperAdmin"); // Ensure this role exists
                }
            }
            return result;
        }

        // --- Login Method ---
        public async Task<AuthResultDto> LoginAsync(LoginRequestDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Invalid credentials." } };
            }

            // Check if email is confirmed
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Email not confirmed. Please check your inbox for the confirmation link." } };
            }

            // Check account status
            if (user.Status == UserStatus.Deactivated)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Your account has been deactivated. Please contact support." } };
            }
            if (user.UserType == UserType.ServiceProvider && user.Status == UserStatus.Pending)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Your Service Provider account is pending approval from an administrator." } };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var token = await _jwtTokenGenerator.GenerateTokenAsync(user);
                _logger.LogInformation($"User {user.Email} logged in successfully.");
                return new AuthResultDto { Success = true, Token = token, UserId = user.Id, Message = "Login successful." };
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning($"User {user.Email} account locked out.");
                return new AuthResultDto { Success = false, Errors = new[] { $"Account locked out. Please try again in {user.LockoutEnd?.Subtract(DateTimeOffset.UtcNow).Minutes} minutes." } };
            }

            _logger.LogWarning($"Login failed for {user.Email}: Invalid password.");
            return new AuthResultDto { Success = false, Errors = new[] { "Invalid credentials." } };
        }

        // --- Email Confirmation ---
        public async Task<AuthResultDto> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "User not found." } };
            }

            var decodedToken = Uri.UnescapeDataString(token); // Ensure token is unescaped
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                // Update UserStatus after email confirmation
                //|| user.UserType == UserType.SuperAdmin
                if (user.UserType == UserType.Patient )
                {
                    user.Status = UserStatus.Active; // Patients/SuperAdmins become active immediately
                }
                else if (user.UserType == UserType.ServiceProvider)
                {
                    user.Status = UserStatus.Pending; // Service Providers go to Pending for admin approval
                }
                await _userManager.UpdateAsync(user);

                _logger.LogInformation($"Email confirmed for user {user.Email}.");
                return new AuthResultDto { Success = true, Message = "Email confirmed successfully. You can now log in." };
            }

            _logger.LogError($"Email confirmation failed for user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
        }

        // --- Password Management ---
        public async Task<AuthResultDto> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                _logger.LogWarning($"Forgot password request for non-existent or unconfirmed email: {email}");
                return new AuthResultDto { Success = true, Message = "If an account with that email exists, a password reset link has been sent." };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"YOUR_FRONTEND_APP_URL/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendEmailAsync(user.Email, "Reset Your Password", $"Please reset your password by clicking this link: <a href='{resetLink}'>link</a>");

            _logger.LogInformation($"Password reset link sent to {user.Email}.");
            return new AuthResultDto { Success = true, Message = "If an account with that email exists, a password reset link has been sent." };
        }

        public async Task<AuthResultDto> ResetPasswordAsync(ResetPasswordRequestDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Invalid user or token." } };
            }

            var decodedToken = Uri.UnescapeDataString(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

            if (result.Succeeded)
            {
                _logger.LogInformation($"Password reset successfully for user {user.Email}.");
                return new AuthResultDto { Success = true, Message = "Password has been reset successfully. You can now log in with your new password." };
            }

            _logger.LogError($"Password reset failed for user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
        }

        public async Task<AuthResultDto> ChangePasswordAsync(string userId, ChangePasswordRequestDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "User not found." } };
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                await _signInManager.SignOutAsync(); // Sign out after password change for security
                _logger.LogInformation($"Password changed successfully for user {user.Email}.");
                return new AuthResultDto { Success = true, Message = "Password changed successfully. Please log in with your new password." };
            }

            _logger.LogError($"Password change failed for user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
        }

        public async Task<AuthResultDto> ChangeEmailAsync(string userId, ChangeEmailRequestDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "User not found." } };
            }

            // Verify current password before changing email
            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!passwordCheck)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Incorrect current password." } };
            }

            // Check if new email is already taken
            var existingUserWithNewEmail = await _userManager.FindByEmailAsync(model.NewEmail);
            if (existingUserWithNewEmail != null && existingUserWithNewEmail.Id != user.Id)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "New email address is already taken." } };
            }

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
            var confirmationLink = $"YOUR_FRONTEND_APP_URL/confirm-new-email?userId={user.Id}&newEmail={Uri.EscapeDataString(model.NewEmail)}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(model.NewEmail, "Confirm Your New Email Address", $"Please confirm your new email by clicking this link: <a href='{confirmationLink}'>link</a>");

            // Mark user's EmailConfirmed to false temporarily if you want to force re-confirmation for login
            user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);
            await _signInManager.SignOutAsync(); // Sign out user to force re-login after confirmation

            _logger.LogInformation($"Change email initiated for user {user.Email} to {model.NewEmail}. Confirmation email sent.");
            return new AuthResultDto { Success = true, Message = "A confirmation email has been sent to your new email address. Please confirm it to complete the change and re-login." };
        }

        // --- Admin User Status Management (Common for all users) ---
        public async Task<bool> UpdateUserStatusAsync(string userId, UserStatus newStatus)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"Attempted to update status for non-existent user ID: {userId}");
                return false;
            }

            user.Status = newStatus;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError($"Failed to update status for user {userId} to {newStatus}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            else
            {
                _logger.LogInformation($"User {userId} status updated to {newStatus}.");
            }
            return result.Succeeded;
        }
    }
}