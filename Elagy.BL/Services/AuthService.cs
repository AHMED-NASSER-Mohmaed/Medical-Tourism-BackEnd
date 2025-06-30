using AutoMapper;
using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Files;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.Helpers;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IEmailService = Elagy.Core.Helpers.IEmailService;
using IJwtTokenGenerator = Elagy.Core.Helpers.IJwtTokenGenerator;
using ServiceProvider= Elagy.Core.Entities.ServiceProvider;

namespace Elagy.BL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IFileStorageService _imageStorage;

        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IEmailService emailService,
            IJwtTokenGenerator jwtTokenGenerator,
            IFileStorageService imageStorage,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _imageStorage = imageStorage;
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

            var user = createUser(); // Create specific user type (Patient, ServiceProvider, SuperAdmin) -- builder
            _mapper.Map(model, user); // Map common properties
            user.UserType = userType;
            user.Status = Status.EmailUnconfirmed; // Initial status

            var result = await _userManager.CreateAsync(user, model.Password);

            //if identity faild to create a new role
            if (!result.Succeeded)
            {
                _logger.LogError($"User creation failed for {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }

            //after creating successfully 
            //we need to the role to the calim 

            string roleName = user.UserType.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogError($"Role '{roleName}' does not exist. Cannot assign user '{user.Email}' to it. Ensure roles are seeded.");
                await _userManager.DeleteAsync(user);
                return new AuthResultDto { Success = false, Errors = new[] { $"Configuration Error: Role '{roleName}' not found." } };
            }


            var roleResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!roleResult.Succeeded)
            {
                _logger.LogError($"Failed to add user {user.Email} to role {roleName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                await _userManager.DeleteAsync(user);
                return new AuthResultDto { Success = false, Errors = new[] { $"Failed to assign user to role '{roleName}'." } };
            }


            //addd user type (optianal)


            var userTypeClaim = new Claim("UserType", user.UserType.ToString());
            var addUserTypeClaimResult = await _userManager.AddClaimAsync(user, userTypeClaim);
            if (!addUserTypeClaimResult.Succeeded)
            {
                _logger.LogWarning($"Failed to add UserType claim '{user.UserType}' to user {user.Email}: {string.Join(", ", addUserTypeClaimResult.Errors.Select(e => e.Description))}. Registration will proceed without this claim.");
                // Decide if this failure is critical enough to rollback. For UserType, usually not.
                //not necessarally for roleback
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

        public async Task<AuthResultDto> RegisterHotelProviderAsync(HotelAssetRegistrationRequestDto model,List<IFormFile> files)
        {
            return await RegisterServiceProviderAsync<HotelAsset, HotelAssetRegistrationRequestDto>(model, AssetType.Hotel, () => new HotelAsset(),files);
        }

        public async Task<AuthResultDto> RegisterHospitalProviderAsync(HospitalAssetRegistrationRequestDto model,List<IFormFile> files)
        {
            return await RegisterServiceProviderAsync<HospitalAsset, HospitalAssetRegistrationRequestDto>(model, AssetType.Hospital, () => new HospitalAsset(),files);
        }

        public async Task<AuthResultDto> RegisterCarRentalProviderAsync(CarRentalAssetRegistrationRequestDto model, List<IFormFile> files)
        {
            return await RegisterServiceProviderAsync<CarRentalAsset, CarRentalAssetRegistrationRequestDto>(model, AssetType.CarRental, () => new CarRentalAsset(),files);
        }

        private async Task HandelDeleting(List<FileUploadResponseDto> Files)
        {
            var list = new List<string> { };

            if(Files[0].Success)
                list.Add(Files[0].Id.ToString());

            if (Files[1].Success)
                list.Add(Files[0].Id.ToString());

            await _imageStorage.DeleteMultipleFilesAsync(list);
        }


        private async Task<AuthResultDto> RegisterServiceProviderAsync<TAsset, TDto>(TDto model, AssetType assetType, Func<TAsset> createAsset,  List<IFormFile> files)
            where TAsset : Asset
            where TDto : BaseAssetRegistrationRequestDto
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);


            if (userExists != null)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Email already registered." } };
            }

           
            var Result = await _imageStorage.UploadMultipleFilesAsync(files);

            //if over all success
            if (!Result.OverallSuccess)
                    HandelDeleting(Result.UploadResults);               
                
 
            var serviceProvider = _mapper.Map<ServiceProvider>(model);
            serviceProvider.NationalURL = Result.UploadResults[0].Url;
            serviceProvider.UserType = UserType.ServiceProvider;
            serviceProvider.NationalFeildId = Result.UploadResults[0].Id;
            serviceProvider.Status = Status.EmailUnconfirmed;


 
            var result = await _userManager.CreateAsync(serviceProvider, model.Password);

            if (!result.Succeeded)
            {
                //handel Delete for Docs
                HandelDeleting(Result.UploadResults);

                _logger.LogError($"Service Provider creation failed for {serviceProvider.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }

            
            string roleName = MapAssetTypeToUserType(assetType).ToString();

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogError($"Role '{roleName}' does not exist. Cannot assign user '{serviceProvider.Email}' to it. Ensure roles are seeded.");
                await _userManager.DeleteAsync(serviceProvider);
                HandelDeleting(Result.UploadResults);
                return new AuthResultDto { Success = false, Errors = new[] { $"Configuration Error: Role '{roleName}' not found." } };
            }


            var roleResult = await _userManager.AddToRoleAsync(serviceProvider, roleName);

            if (!roleResult.Succeeded)
            {
                _logger.LogError($"Failed to add user {serviceProvider.Email} to role {roleName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                await _userManager.DeleteAsync(serviceProvider);
                HandelDeleting(Result.UploadResults);
                return new AuthResultDto { Success = false, Errors = new[] { $"Failed to assign user to role '{roleName}'." } };
            }


             
            var userTypeClaim = new Claim("UserType", serviceProvider.UserType.ToString());
            var addUserTypeClaimResult = await _userManager.AddClaimAsync(serviceProvider, userTypeClaim);
            if (!addUserTypeClaimResult.Succeeded)
            {
                _logger.LogWarning($"Failed to add UserType claim '{serviceProvider.UserType}' to user {serviceProvider.Email}: {string.Join(", ", addUserTypeClaimResult.Errors.Select(e => e.Description))}. Registration will proceed without this claim.");
                 
            }





            // Create the associated ServiceAsset
            var serviceAsset = createAsset();

            _mapper.Map(model, serviceAsset); // Map common asset properties from DTO

            serviceAsset.Id = serviceProvider.Id; // Set AssetId to match ServiceProvider's Id (shared PK)
            serviceAsset.ServiceProvider = serviceProvider; // Link to the provider
            serviceAsset.AssetType = assetType;
            serviceAsset.CredentialDocURL= Result.UploadResults[1].Url;
            serviceAsset.CredentialDocId= Result.UploadResults[1].Id;



            try
            {

                await _unitOfWork.ServiceAssets.AddAsync(serviceAsset);
                await _unitOfWork.CompleteAsync(); // Save the asset and link
            }
            catch (DbUpdateException dbEx)
            {
                //handelDelete
                HandelDeleting(Result.UploadResults);

                _logger.LogError(dbEx, $"Failed to create ServiceAsset for {serviceProvider.Email}. Rolling back user creation.");
                await _userManager.DeleteAsync(serviceProvider); // Rollback user creation
                return new AuthResultDto { Success = false, Errors = new[] { "Failed to create associated asset. Please try again." } };
            }
            catch (Exception ex)
            {
                //handelDelete
                HandelDeleting(Result.UploadResults);

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
                // Do not reveal if the email exists or not
                return new AuthResultDto { Success = false, Errors = new[] { "Invalid credentials." } };
            }

            // If the email is not confirmed, check if the user's current Email matches the login email
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                // If the user's email was changed by admin, don't show "Email not confirmed"
                if (string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    return new AuthResultDto { Success = false, Errors = new[] { "Email not confirmed. Please check your inbox for the confirmation link." } };
                }
                // If the email does not match, treat as invalid credentials
                return new AuthResultDto { Success = false, Errors = new[] { "Invalid credentials." } };
            }

            // Check account status 
            if (user.Status == Status.Deactivated)
            {
                return new AuthResultDto { Success = false, Errors = new[] { "Your account has been deactivated. Please contact support." } };
            }
            if (user.UserType == UserType.ServiceProvider && user.Status == Status.Pending)
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
                if (user.UserType == UserType.Patient)
                {
                    user.Status = Status.Active; // Patients/SuperAdmins become active immediately
                }
                else if (user.UserType == UserType.ServiceProvider)
                {
                    user.Status = Status.Pending; // Service Providers go to Pending for admin approval
                }
                await _userManager.UpdateAsync(user);

                _logger.LogInformation($"Email confirmed for user {user.Email}.");
                return new AuthResultDto { Success = true, Message = "Email confirmed successfully. You can now log in." };
            }

            _logger.LogError($"Email confirmation failed for user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
        }



        // --- Password Management ---


        /// <summary>
        /// Confirms a new email address after an admin-initiated change.
        /// Includes extensive logging for debugging token issues.
        /// </summary>
        public async Task<AuthResultDto> ConfirmNewEmailAsync(string userId, string newEmail, string token)
        {
            var currentUtcTimeConf = DateTime.UtcNow; // Capture time at confirmation start

            _logger.LogInformation($"[{currentUtcTimeConf:yyyy-MM-dd HH:mm:ss.fff UTC} CONF] ConfirmNewEmailAsync: Received - userId: '{userId}'");
            _logger.LogInformation($"[{currentUtcTimeConf:yyyy-MM-dd HH:mm:ss.fff UTC} CONF] Received - rawNewEmail: '{newEmail}' (Length: {newEmail.Length}) - Hex: {BitConverter.ToString(Encoding.UTF8.GetBytes(newEmail))}");
            _logger.LogInformation($"[{currentUtcTimeConf:yyyy-MM-dd HH:mm:ss.fff UTC} CONF] Received - rawToken: '{token}' (Length: {token.Length}) - Hex: {BitConverter.ToString(Encoding.UTF8.GetBytes(token))}");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"[{currentUtcTimeConf:yyyy-MM-dd HH:mm:ss.fff UTC} CONF] User not found for ID: '{userId}'.");
                return new AuthResultDto { Success = false, Errors = new[] { "User not found." } };
            }

            // Log user's security stamp at the point of confirmation
            var securityStampAtConfirmation = await _userManager.GetSecurityStampAsync(user);
            _logger.LogInformation($"[{currentUtcTimeConf:yyyy-MM-dd HH:mm:ss.fff UTC} CONF] User '{user.Email}' (ID: '{user.Id}') security stamp at confirmation: '{securityStampAtConfirmation}'");


            var decodedToken = Uri.UnescapeDataString(token);
            var decodedNewEmail = Uri.UnescapeDataString(newEmail); // Unescape the newEmail parameter as well!

            _logger.LogInformation($"[{currentUtcTimeConf:yyyy-MM-dd HH:mm:ss.fff UTC} CONF] Decoded - decodedNewEmail: '{decodedNewEmail}' (Length: {decodedNewEmail.Length}) - Hex: {BitConverter.ToString(Encoding.UTF8.GetBytes(decodedNewEmail))}");
            _logger.LogInformation($"[{currentUtcTimeConf:yyyy-MM-dd HH:mm:ss.fff UTC} CONF] Decoded - decodedToken: '{decodedToken}' (Length: {decodedToken.Length}) - Hex: {BitConverter.ToString(Encoding.UTF8.GetBytes(decodedToken))}");


            // --- THE CRITICAL CALL TO VALIDATE TOKEN AND CHANGE EMAIL ---
            var result = await _userManager.ChangeEmailAsync(user, decodedNewEmail, decodedToken);

            if (result.Succeeded)
            {
                // Optionally, update user status here if needed, similar to initial confirmation
                if (user.Status == Status.EmailUnconfirmed || user.Status == Status.Pending) // Allow confirmation from these states
                {
                    user.Status = Status.Active; // Or whatever status means confirmed in your app
                    await _userManager.UpdateAsync(user);
                    _logger.LogInformation($"[{currentUtcTimeConf:yyyy-MM-dd HH:mm:ss.fff UTC} CONF] User status updated to '{user.Status}' for '{user.Email}'.");
                }

                _logger.LogInformation($"[{currentUtcTimeConf:yyyy-MM-dd HH:mm:ss.fff UTC} CONF] New email '{decodedNewEmail}' confirmed successfully for user '{user.Email}'.");
                return new AuthResultDto { Success = true, Message = "Your new email address has been confirmed successfully." };
            }
            else
            {
                // Log the specific Identity errors for "Invalid token."
                var errors = result.Errors.Select(e => $"Code: {e.Code}, Description: {e.Description}").ToList();
                _logger.LogError($"[{currentUtcTimeConf:yyyy-MM-dd HH:mm:ss.fff UTC} CONF] New email confirmation failed for user '{user.Email}' (new: '{decodedNewEmail}'). Errors: {string.Join("; ", errors)}");

                return new AuthResultDto { Success = false, Errors = result.Errors.Select(e => e.Description) };
            }
        }

         
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
            var resetLink = $"http://localhost:4200/auth/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendEmailAsync(user.Email, "Reset Your Password", $"Please reset your password by clicking this link: <a href='{resetLink}'>link</a>");

            _logger.LogInformation($"Password reset link sent to {user.Email}.");
            return new AuthResultDto { Success = true, Message = "If an account with that email exists, a password reset link has been sent." };
        }

        public async Task<AuthResultDto> ResetPasswordAsync(string userId,ResetPasswordRequestDto model)
        {
            var user = await _userManager.FindByIdAsync(userId);
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
            var confirmationLink = $"http://localhost:4200/auth/confirm-new-email?userId={user.Id}&newEmail={Uri.EscapeDataString(model.NewEmail)}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(model.NewEmail, "Confirm Your New Email Address", $"Please confirm your new email by clicking this link: <a href='{confirmationLink}'>link</a>");

            // Mark user's EmailConfirmed to false temporarily if you want to force re-confirmation for login
            user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);
            await _signInManager.SignOutAsync(); // Sign out user to force re-login after confirmation

            _logger.LogInformation($"Change email initiated for user {user.Email} to {model.NewEmail}. Confirmation email sent.");
            return new AuthResultDto { Success = true, Message = "A confirmation email has been sent to your new email address. Please confirm it to complete the change and re-login." };
        }

        // --- Admin User Status Management (Common for all users) ---
        public async Task<bool> UpdateUserStatusAsync(string userId, Status newStatus)
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


        private RoleApps MapAssetTypeToUserType(AssetType assetType)
        {
            return assetType switch
            {
                AssetType.Hospital => RoleApps.HospitalServiceProvider,
                AssetType.Hotel => RoleApps.HotelServiceProvider,
                AssetType.CarRental => RoleApps.CarRentalServiceProvider,
                _ => throw new ArgumentOutOfRangeException(nameof(assetType), $"Unsupported AssetType for role mapping: {assetType}")
            };
        }


    }
}