using AutoMapper;
using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.Helpers; // Required for Guid and DateTime.UtcNow
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Required for .Include() and .ToListAsync()
using Microsoft.Extensions.Logging; // Add this line if it's missing
using System.Text;
using ServiceProvider = Elagy.Core.Entities.ServiceProvider; // Ensure this is the correct namespace for ServiceProvider

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
            if (user.Status == Status.Active)
            {
                return new AuthResultDto { Success = true, Message = "User is already active." };
            }

            user.Status = Status.Active;
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
            if (user.Status == Status.Deactivated)
            {
                return new AuthResultDto { Success = true, Message = "User is already deactivated." };
            }

            user.Status = Status.Deactivated;
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
            if (provider.Status == Status.Active)
            {
                return new AuthResultDto { Success = true, Message = "Service Provider is already approved and active." };
            }

            provider.Status = Status.Active;
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
            provider.Status = Status.Deactivated;
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

        private IQueryable<User> ApplyUserFilters(IQueryable<User> query, string? searchQuery, Status? status)
        {
            // --- 1. Apply UserStatus Filter (Independent) ---
            // This filter is applied regardless of whether a search query is present.
            if (status.HasValue)
            {
                query = query.Where(u => u.Status == status.Value);
            }

            // --- 2. Apply Search Term Filter (Independent) ---
            // This filter is applied only if a search query is present.
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string lowerSearchQuery = searchQuery.Trim().ToLower(); // Trim and convert to lowercase once.

                // Combine search conditions with OR.
                // Assuming properties like Email, FirstName, LastName, Id are non-nullable strings.
                query = query.Where(u =>
                                       u.Email.ToLower().Contains(lowerSearchQuery) ||
                                       u.FirstName.ToLower().Contains(lowerSearchQuery) ||
                                       u.LastName.ToLower().Contains(lowerSearchQuery) ||
                                       u.PhoneNumber.ToLower().Contains(lowerSearchQuery) // Often useful to search phone numbers too
                                   );
            }

            // Optional: Apply default sorting here if not done elsewhere for consistent pagination.
            // query = query.OrderBy(u => u.Email); // Example sorting

            return query;
        }

        private IQueryable<ServiceProvider> ApplyServiceProviderFilters(IQueryable<ServiceProvider> query, string? searchQuery, Status? userStatus)
        {
            // Ensure ServiceAsset is included for filtering on its properties, and for later mapping
            query = query.Include(sp => sp.ServiceAsset);

            // --- 1. Apply UserStatus Filter (Independent of search query) ---
            if (userStatus.HasValue)
            {
                query = query.Where(sp => sp.Status == userStatus.Value);
            }

            // --- 2. Apply Search Term Filter ---
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                string lowerSearchQuery = searchQuery.Trim().ToLower(); // Trim any whitespace

                 
                query = query.Where(sp =>
                                        sp.Email.ToLower().Contains(lowerSearchQuery) ||
                                        sp.FirstName.ToLower().Contains(lowerSearchQuery) ||
                                        sp.LastName.ToLower().Contains(lowerSearchQuery) ||
                                        sp.Phone.ToLower().Contains(lowerSearchQuery) || 
                                        sp.ServiceAsset.Name.ToLower().Contains(lowerSearchQuery)
                                   );
            }


            return query;
        }

        public async Task<PagedResponseDto<PatientDto>> GetPatientsForAdminDashboardAsync(PaginationParameters RquestParams)
        {

            // 1. Start with the base query for Patients
            var query = _unitOfWork.Patients.AsQueryable().OfType<Patient>(); // Use AsQueryable()

            // 2. Apply filters (cast to User for filtering, then back to Patient)
            // Ensure ApplyUserFilters handles the filtering correctly on IQueryable<User>
            query = ApplyUserFilters(query.Cast<User>(), RquestParams.SearchTerm, RquestParams.UserStatus).OfType<Patient>(); // Apply filters, cast back to Patient

            // --- CRITICAL STEP: Get the total count AFTER all filters are applied ---
            var totalCount = await query.CountAsync(); // calculation against DB


            // calculation against DB
            var res = await query  
           .Skip((RquestParams.PageNumber - 1) * RquestParams.PageSize)
           .Take(RquestParams.PageSize)
           .ToListAsync(); // This executes the query for the current page



            // 4. Map the results to DTOs
            var MappedResult = _mapper.Map<List<PatientDto>>(res); // Ensure you map to List<T> if PagedResponseDto expects a List



            // 5. Return the PagedResponseDto, which calculates TotalPages internally
            return new PagedResponseDto<PatientDto>(MappedResult, totalCount, RquestParams.PageNumber, RquestParams.PageSize);
        }

 
        public async Task<PagedResponseDto<HotelProviderProfileDto>> GetHotelProvidersForAdminDashboardAsync(PaginationParameters requestParams) 
        {
            var query = _unitOfWork.ServiceProviders.AsQueryable().OfType<ServiceProvider>();

             
            query = query.Include(sp => sp.ServiceAsset);

            query = query.Include(sp => sp.Governorate)
             .ThenInclude(g => g.Country);

            query = query.Include(sp => sp.ServiceAsset)
            .ThenInclude(a => a.Governate)
            .ThenInclude(g => g.Country);


            query = query.Where(sp => sp.ServiceAsset.AssetType == AssetType.Hotel);

            if(requestParams.FilterGovernorateId.HasValue)
            {
                query=query.Where(s=>s.ServiceAsset.GovernateId==requestParams.FilterGovernorateId.Value);
            }
            query = ApplyServiceProviderFilters(query,requestParams.SearchTerm ,requestParams.UserStatus);

            // --- CRITICAL STEP: Get the total count AFTER all filters are applied ---
            var totalCount = await query.CountAsync();

            // 4. Apply pagination (Skip and Take)
            var pagedServiceProviders = await query
                .Skip((requestParams.PageNumber - 1) * requestParams.PageSize)
                .Take(requestParams.PageSize)
                .ToListAsync(); // This executes the query for the current page

            // 5. Map the results to DTOs
            // Make sure your AutoMapper configuration maps ServiceProvider to HotelProviderProfileDto.
            var MappedResult = _mapper.Map<List<HotelProviderProfileDto>>(pagedServiceProviders);

            // 6. Return the PagedResponseDto, which calculates TotalPages internally
            return new PagedResponseDto<HotelProviderProfileDto>(
                MappedResult,
                totalCount ,
                requestParams.PageNumber,
                requestParams.PageSize
            );
        }

        public async Task<PagedResponseDto<HospitalProviderProfileDto>> GetHospitalProvidersForAdminDashboardAsync(PaginationParameters requestParams) 
        {

            IQueryable<ServiceProvider> query = _unitOfWork.ServiceProviders
                                                .AsQueryable()
                                                .OfType<ServiceProvider>();

            query = query.Include(sp => sp.Governorate)
                .ThenInclude(g => g.Country);

            query= query.Include(sp => sp.ServiceAsset)
                .ThenInclude(a => a.Governate)
                .ThenInclude(g => g.Country)
                .Where(sp => sp.ServiceAsset != null && sp.ServiceAsset.AssetType == AssetType.Hospital);


            if (requestParams.SpecialtyId.HasValue)
            {
                 query = query.Where(sp =>
                    ((HospitalAsset)sp.ServiceAsset) 
                    .HospitalSpecialties.Any(hs => hs.SpecialtyId == requestParams.SpecialtyId.Value)
                );
            }

            if (requestParams.FilterGovernorateId.HasValue)
            {
                query = query.Where(sp => sp.ServiceAsset.GovernateId == requestParams.FilterGovernorateId.Value);
            }

            query = ApplyServiceProviderFilters(query, requestParams.SearchTerm, requestParams.UserStatus);

            var totalCount = await query.CountAsync();

 
            var pagedQuery =await query
                .Skip((requestParams.PageNumber - 1) * requestParams.PageSize)
                .Take(requestParams.PageSize)
                .ToListAsync();


            var MappedResult = _mapper.Map<List<HospitalProviderProfileDto>>(pagedQuery);

             return new PagedResponseDto<HospitalProviderProfileDto>(
                MappedResult,
                totalCount ,
                requestParams.PageNumber,
                requestParams.PageSize
            );
        }

        public async Task<PagedResponseDto<CarRentalProviderProfileDto>> GetCarRentalProvidersForAdminDashboardAsync(PaginationParameters requestParams)  
        {
            // 1. Start with the base query for ServiceProviders
            IQueryable<ServiceProvider> query = _unitOfWork.ServiceProviders
                .AsQueryable()
                .OfType<ServiceProvider>(); // Ensures we're working with ServiceProvider entities

            // 2. Apply the fixed filter for CarRental assets
            //    Include ServiceAsset first, as it's needed for this filter (and likely for mapping).
            query = query
                .Include(sp => sp.ServiceAsset) // Eager load ServiceAsset if needed for AssetType filter and mapping
                .Where(sp => sp.ServiceAsset != null && sp.ServiceAsset.AssetType == AssetType.CarRental); // Filter for CarRental assets


            // 3. Apply additional dynamic filters (searchQuery, userStatus) using the helper function
            query = ApplyServiceProviderFilters(query,requestParams.SearchTerm , requestParams.UserStatus );

            // --- CRITICAL STEP: Get the total count AFTER all filters are applied ---
            var totalCount = await query.CountAsync();

            // 4. Apply pagination (Skip and Take)
            var pagedServiceProviders = await query
                .Skip((requestParams.PageNumber - 1) * requestParams.PageSize)
                .Take(requestParams.PageSize)
                .ToListAsync(); // This executes the query for the current page

            // 5. Map the results to DTOs
            // Make sure your AutoMapper configuration maps ServiceProvider to CarRentalProviderProfileDto.
            var MappedResult = _mapper.Map<List<CarRentalProviderProfileDto>>(pagedServiceProviders);

            // 6. Return the PagedResponseDto, which calculates TotalPages internally
            return new PagedResponseDto<CarRentalProviderProfileDto>(
                MappedResult,
                totalCount ,
                requestParams.PageNumber,
                requestParams.PageSize
            );
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
        /*  public async Task<AuthResultDto> AdminChangeUserEmailAsync(string userId, string newEmail)
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
  */



        /// <summary>
        /// Initiates an email address change for a user by an administrator.
        /// Sends a confirmation link to the new email and a notification to the old email.
        /// Includes extensive logging for debugging token issues.
        /// </summary>
        /// <param name="userId">The ID of the user whose email is being changed.</param>
        /// <param name="newEmail">The new email address to assign to the user.</param>
        /// <returns>AuthResultDto indicating success or failure of the initiation.</returns>
        public async Task<AuthResultDto> AdminChangeUserEmailAsync(string userId, string newEmail)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] AdminChangeUserEmailAsync: User not found for ID '{userId}'.");
                return new AuthResultDto { Success = false, Errors = new[] { "User not found." } };
            }

            string oldEmail = user.Email;
            var currentUtcTimeGen = DateTime.UtcNow;

            _logger.LogInformation($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] AdminChangeUserEmailAsync: Initiating change for User ID: '{user.Id}'");
            _logger.LogInformation($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] OLD Email (User Object): '{oldEmail}' (Length: {oldEmail.Length}) - Hex: {BitConverter.ToString(Encoding.UTF8.GetBytes(oldEmail))}");
            _logger.LogInformation($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] NEW Email (Input Param): '{newEmail}' (Length: {newEmail.Length}) - Hex: {BitConverter.ToString(Encoding.UTF8.GetBytes(newEmail))}");

            // Check if the new email is already taken by another user
            var existingUserWithNewEmail = await _userManager.FindByEmailAsync(newEmail);
            if (existingUserWithNewEmail != null && existingUserWithNewEmail.Id != user.Id)
            {
                _logger.LogWarning($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] New email '{newEmail}' is already taken by another user.");
                return new AuthResultDto { Success = false, Errors = new[] { "New email address is already taken by another user." } };
            }

            // Log user's security stamp BEFORE token generation and BEFORE any DB updates
            var securityStampAtTokenGen = await _userManager.GetSecurityStampAsync(user);
            _logger.LogInformation($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] Security Stamp AT TOKEN GENERATION: '{securityStampAtTokenGen}'");

            // --- CRITICAL STEP: Generate the token *before* any updates to the user object in the database ---
            // This token is generated against the *current* security stamp of the user.
            var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);
            _logger.LogInformation($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] Generated Token (raw from UserManager): '{token}' (Length: {token.Length}) - Hex: {BitConverter.ToString(Encoding.UTF8.GetBytes(token))}");

            // The confirmationLink uses the 'token' that was generated *before* any potential DB updates to the user object.
            var confirmationLink = $"YOUR_FRONTEND_APP_URL/confirm-new-email?userId={user.Id}&newEmail={Uri.EscapeDataString(newEmail)}&token={Uri.EscapeDataString(token)}";
            _logger.LogInformation($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] Full Confirmation Link generated: '{confirmationLink}'");

            // --- Step 1: Send Confirmation to the NEW Email Address ---
            _logger.LogInformation($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] Attempting to send confirmation email to new address: '{newEmail}'.");
            try
            {
                if (_emailService == null)
                {
                    _logger.LogError($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] _emailService is NULL. Confirmation email cannot be sent.");
                    return new AuthResultDto { Success = false, Errors = new[] { "Email service is not available." } };
                }

                await _emailService.SendEmailAsync(
                    newEmail,
                    "Confirm Your New Email Address (Admin Initiated)",
                    $"Your email address for [Your App Name] was changed by an administrator. " +
                    $"Please confirm your new email by clicking this link: <a href='{confirmationLink}'>link</a>. " +
                    $"If you did not request this change, please contact support immediately."
                );
                _logger.LogInformation($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] Confirmation email sent successfully (or completed execution) to new address: '{newEmail}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] EXCEPTION during sending confirmation email to new address '{newEmail}' for user '{userId}'.");
                return new AuthResultDto { Success = false, Errors = new[] { "Failed to send confirmation email to the new address due to an error." } };
            }

            // --- Step 2: Send Notification to the OLD Email Address (Crucial Security Step) ---
            if (!string.Equals(oldEmail, newEmail, StringComparison.OrdinalIgnoreCase)) // Only notify if email actually changed
            {
                _logger.LogInformation($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] Attempting to send notification email to old address: '{oldEmail}'.");
                try
                {
                    if (_emailService == null)
                    {
                        _logger.LogError($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] _emailService is NULL. Old email notification cannot be sent.");
                    }
                    else
                    {
                        await _emailService.SendEmailAsync(
                            oldEmail,
                            "Account Email Address Changed (Action Required if Unauthorized)",
                            $"This is an important security notification for your account with [Your App Name]. " +
                            $"Your email address has been changed from '{oldEmail}' to '{newEmail}' by an administrator. " +
                            $"If you did not authorize this change, please contact our support team immediately at [Your Support Contact Info or Link]."
                        );
                        _logger.LogInformation($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] Notification email sent successfully (or completed execution) to old address: '{oldEmail}'.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] EXCEPTION during sending notification email to old address '{oldEmail}' for user '{userId}'. This is a critical security warning.");
                }
            }

 

            _logger.LogInformation($"[{currentUtcTimeGen:yyyy-MM-dd HH:mm:ss.fff UTC} GEN] AdminChangeUserEmailAsync completed for user {oldEmail} -> {newEmail}. Confirmation email sent to new, notification to old. User's DB state (Email and EmailConfirmed) is awaiting confirmation.");
            return new AuthResultDto
            {
                Success = true,
                Message = "Email change initiated. Confirmation email sent to the new address. " +
                          "A notification has also been sent to the old email address for security. " +
                          "The user needs to confirm the new email to activate it."
            };
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

/*        public async Task<AuthResultDto> AdminSetAssetVerificationStatusAsync(string assetId, VerificationStatus status, string notes = null)
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
        }*/

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