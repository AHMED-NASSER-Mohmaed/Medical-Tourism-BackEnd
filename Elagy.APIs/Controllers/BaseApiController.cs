using Microsoft.AspNetCore.Mvc;
using Elagy.Core.DTOs.Shared; // For AuthResultDto
using System.Linq;
using System.Security.Claims; // For getting UserId from HttpContext

namespace Elagy.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Base route for all controllers
    public abstract class BaseApiController : ControllerBase
    {
        // Helper method to return AuthResultDto consistently
        protected ActionResult HandleAuthResult(AuthResultDto result)
        {
            if (result.Success)
            {
                return Ok(result);
            }
            // For common errors, you might want to return different HTTP status codes
            // e.g., 400 Bad Request for validation errors, 401 Unauthorized for login issues, etc.
            if (result.Errors != null && result.Errors.Any())
            {
                // This is a simple generic bad request for errors
                return BadRequest(new { result.Success, result.Message, result.Errors });
            }
            return StatusCode(500, new { result.Success, result.Message, Errors = new[] { "An unexpected error occurred." } });
        }

        // Helper method to get the current logged-in user's ID
        protected string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        protected string GetCurrentUserEmial()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }


        /// <summary>
        /// Validates uploaded files for count and size.
        /// </summary>
        /// <param name="files">List of uploaded files</param>
        /// <param name="expectedCount">Expected number of files</param>
        /// <param name="maxFileSize">Maximum allowed file size in bytes</param>
        /// <returns>Validation result with success flag and error messages</returns>
        protected (bool Success, List<string> Errors) ValidateFiles(List<IFormFile> files, int expectedCount, long maxFileSize)
        {
            var errors = new List<string>();

            if (files == null || files.Count != expectedCount)
            {
                errors.Add($"Exactly {expectedCount} files are required.");
            }
            else
            {
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i].Length > maxFileSize)
                    {
                        errors.Add($"File {i + 1} exceeds the maximum allowed size of {maxFileSize / (1024 * 1024)}MB.");
                    }
                }
            }

            return (errors.Count == 0, errors);
        }

    }
}