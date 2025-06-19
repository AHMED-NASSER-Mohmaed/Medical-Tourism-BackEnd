using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;  
using System.Threading.Tasks;
using Elagy.Core.IServices;  
using Elagy.Core.DTOs.Files; 
using Microsoft.Extensions.Logging;  
using Microsoft.AspNetCore.Authorization;  
using System.Security.Claims;
using Elagy.Core.Enums;  

namespace Elagy.APIs.Controllers
{

    [Route("profile/[controller]")]
    public abstract class ProfileImageBaseController : BaseApiController  
    {
        protected readonly IImageProfile _profileImageService;
        protected readonly ILogger<ProfileImageBaseController> _logger;

        
        public ProfileImageBaseController(
            IImageProfile profileImageService,
            ILogger<ProfileImageBaseController> logger)
        {
            _profileImageService = profileImageService;
            _logger = logger;  
        }

        
        [HttpPut("{userId}/profile-image")]
        [RequestSizeLimit(20 * 1024 * 1024)]
        public async Task<ActionResult<FileUploadResponseDto>> UpdateUserProfileImage(
            string userId,
            [FromForm] IFormFile imageFile)
        {
             string currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

            
            if (!User.IsInRole(RoleApps.SuperAdmin.ToString()) && currentUserId != userId) 
            {
                _logger.LogWarning($"Unauthorized attempt to update profile image for user '{userId}' by user '{currentUserId}'.");
                return Forbid();
            }

            if (imageFile == null || imageFile.Length == 0)
            {
                _logger.LogWarning($"UpdateUserProfileImage (Base): No image file received for user ID '{userId}'.");
                return BadRequest(new FileUploadResponseDto { Success = false, Message = "No image file provided." });
            }

            try
            {
                var result = await _profileImageService.UpdateImageProfile(userId, imageFile);

                if (result.Success)
                {
                    _logger.LogInformation($"UpdateUserProfileImage (Base): Profile image updated successfully for user '{userId}'.");
                    return Ok(result);
                }
                else
                {
                    _logger.LogError($"UpdateUserProfileImage (Base): Failed to update profile image for user '{userId}'. Details: {result.Message}");
                    return StatusCode(500, result);
                }
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, $"UpdateUserProfileImage (Base): Bad request for user '{userId}'.");
                return BadRequest(new FileUploadResponseDto { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, $"UpdateUserProfileImage (Base): Operation error for user '{userId}'.");
                if (ex.Message.Contains("User not found"))
                {
                    return NotFound(new FileUploadResponseDto { Success = false, Message = ex.Message });
                }
                return StatusCode(500, new FileUploadResponseDto { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateUserProfileImage (Base): An unexpected error occurred updating profile image for user '{userId}'.");
                return StatusCode(500, new FileUploadResponseDto { Success = false, Message = "An unexpected error occurred during profile image update." });
            }
        }

        
        [HttpDelete("{userId}/profile-image")]
        public async Task<ActionResult<FileDeletionResponseDto>> DeleteUserProfileImage(string userId)
        {
            string currentUserId = GetCurrentUserId();
            if (currentUserId == null) return Unauthorized();

             if (!User.IsInRole(RoleApps.SuperAdmin.ToString()) && currentUserId != userId)
            {
                _logger.LogWarning($"Unauthorized attempt to delete profile image for user '{userId}' by user '{currentUserId}'.");
                return Forbid();
            }

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new FileDeletionResponseDto { Success = false, Message = "User ID cannot be empty." });
            }

            try
            {
                var result = await _profileImageService.DeleteImageProfile(userId);

                if (result.Success)
                {
                    _logger.LogInformation($"DeleteUserProfileImage (Base): Profile image deleted successfully for user '{userId}'.");
                    return Ok(result);
                }
                else
                {
                    _logger.LogError($"DeleteUserProfileImage (Base): Failed to delete profile image for user '{userId}'. Details: {result.Message}");
                    return StatusCode(500, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteUserProfileImage (Base): An unexpected error occurred deleting profile image for user '{userId}'.");
                return StatusCode(500, new FileDeletionResponseDto { Success = false, Message = "An unexpected error occurred during profile image deletion." });
            }
        }
    }
}