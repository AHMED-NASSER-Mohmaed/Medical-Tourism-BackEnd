using Microsoft.AspNetCore.Mvc;
using Elagy.Core.DTOs.Shared; // For AuthResultDto
using System.Linq;
using System.Security.Claims; // For getting UserId from HttpContext

namespace Elagy.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Base route for all controllers
    public class BaseApiController : ControllerBase
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
    }
}