using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // For IFormFile
using System.Threading.Tasks;
using Elagy.Core.IServices; // For IProfileImage
using Elagy.Core.DTOs.Files; // For FileUploadResponseDto, FileDeletionResponseDto
using Microsoft.Extensions.Logging;
using System.Security.Claims; // For HttpContext.User.FindFirst etc.
using Microsoft.AspNetCore.Authorization; // For [Authorize]

namespace Elagy.APIs.Controllers
{
    [ApiController]
    [Route("api/users")] // Base route for user-related operations
    public class UserProfileController : ControllerBase
    {
        private readonly IImageProfile _profileImageService; // Inject your image profile service
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IImageProfile profileImageService, ILogger<UserProfileController> logger)
        {
            _profileImageService = profileImageService;
            _logger = logger;
        }

        /// <summary>
        /// Updates a user's profile image. Deletes the old image (if any), uploads the new one,
        /// and updates the user's record with the new image details.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile image is being updated.</param>
        /// <param name="imageFile">The new profile image file (expected via form-data).</param>
        /// <returns>FileUploadResponseDto with the new image details.</returns>
        [HttpPut("{userId}/profile-image")] // PUT is semantically appropriate for updating a resource
        // [Authorize] // Consider requiring authentication
        // [Authorize(Policy = "CanManageUserProfileImage")] // Or a specific policy
        public async Task<ActionResult<FileUploadResponseDto>> UpdateUserProfileImage(
            string userId,
            [FromForm] IFormFile imageFile) // Expects the file from form-data
        {
            //Optional: Validate if the authenticated user is allowed to modify this userId's profile
            // For example, ensure the current user (from HttpContext.User) matches userId
            if (!User.IsInRole("Admin") && User.FindFirst(ClaimTypes.NameIdentifier)?.Value != userId)
            {
                return Forbid(); // Or Unauthorized() if no auth scheme is set
            }

            if (imageFile == null || imageFile.Length == 0)
            {
                _logger.LogWarning($"UpdateUserProfileImage: No image file received for user ID '{userId}'.");
                return BadRequest(new FileUploadResponseDto { Success = false, Message = "No image file provided." });
            }

            try
            {
                // Call your service method to handle the image update logic
                var result = await _profileImageService.UpdateImageProfile(userId, imageFile);

                if (result.Success)
                {
                    _logger.LogInformation($"UpdateUserProfileImage: Profile image updated successfully for user '{userId}'.");
                    return Ok(result);
                }
                else
                {
                    // If the service returns a specific failure, reflect it.
                    // This could be due to ImageKit upload failure or database update failure.
                    _logger.LogError($"UpdateUserProfileImage: Failed to update profile image for user '{userId}'. Details: {result.Message}");
                    return StatusCode(500, result); // 500 for server-side processing errors
                }
            }
            catch (ArgumentException ex) // Catches exceptions thrown by your service for invalid input
            {
                _logger.LogError(ex, $"UpdateUserProfileImage: Bad request for user '{userId}'.");
                return BadRequest(new FileUploadResponseDto { Success = false, Message = ex.Message });
            }
            catch (InvalidOperationException ex) // Catches exceptions for user not found, or critical service errors
            {
                _logger.LogError(ex, $"UpdateUserProfileImage: Operation error for user '{userId}'.");
                if (ex.Message.Contains("User not found"))
                {
                    return NotFound(new FileUploadResponseDto { Success = false, Message = ex.Message });
                }
                return StatusCode(500, new FileUploadResponseDto { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UpdateUserProfileImage: An unexpected error occurred while updating profile image for user '{userId}'.");
                return StatusCode(500, new FileUploadResponseDto { Success = false, Message = "An unexpected error occurred during profile image update." });
            }
        }

        /// <summary>
        /// Deletes a user's profile image.
        /// </summary>
        /// <param name="userId">The ID of the user whose profile image is to be deleted.</param>
        /// <returns>FileDeletionResponseDto indicating the outcome.</returns>
        [HttpDelete("{userId}/profile-image")]
        // [Authorize] // Consider requiring authentication
        // [Authorize(Policy = "CanManageUserProfileImage")] // Or a specific policy
        public async Task<ActionResult<FileDeletionResponseDto>> DeleteUserProfileImage(string userId)
        {
            // Optional: Validate if the authenticated user is allowed to delete this userId's profile
            // if (!User.IsInRole("Admin") && User.FindFirst(ClaimTypes.NameIdentifier)?.Value != userId)
            // {
            //     return Forbid();
            // }

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest(new FileDeletionResponseDto { Success = false, Message = "User ID cannot be empty." });
            }

            try
            {
                // Call your service method to handle the image deletion logic
                var result = await _profileImageService.DeleteImageProfile(userId);

                if (result.Success)
                {
                    _logger.LogInformation($"DeleteUserProfileImage: Profile image deleted successfully for user '{userId}'.");
                    return Ok(result); // Returns 200 OK with success message
                    // Alternatively, for a successful DELETE with no content, you might return:
                    // return NoContent(); // Returns 204 No Content
                }
                else
                {
                    _logger.LogError($"DeleteUserProfileImage: Failed to delete profile image for user '{userId}'. Details: {result.Message}");
                    // Reflect the service's specific error message and potentially status
                    return StatusCode(500, result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"DeleteUserProfileImage: An unexpected error occurred while deleting profile image for user '{userId}'.");
                return StatusCode(500, new FileDeletionResponseDto { Success = false, Message = "An unexpected error occurred during profile image deletion." });
            }
        }
    }
}