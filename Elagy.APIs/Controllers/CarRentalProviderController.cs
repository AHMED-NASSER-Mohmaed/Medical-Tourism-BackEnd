using Elagy.Core.DTOs.User;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // For [Authorize] attribute
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    // Protect this controller, allowing only ServiceProviders that are CarRental type
    [Authorize(Roles = "ServiceProvider")] // Assuming a generic ServiceProvider role for all providers
    public class CarRentalProviderController : BaseApiController
    {
        private readonly ICarRentalProviderService _carRentalProviderService;

        public CarRentalProviderController(ICarRentalProviderService carRentalProviderService)
        {
            _carRentalProviderService = carRentalProviderService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<CarRentalProviderProfileDto>> GetProfile()
        {
            var userId = GetCurrentUserId(); // Get current user's ID from token
            if (userId == null) return Unauthorized();

            var profile = await _carRentalProviderService.GetCarRentalProviderProfileAsync(userId);
            if (profile == null)
            {
                return NotFound("Car Rental provider profile not found or user is not a Car Rental provider.");
            }
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<CarRentalProviderProfileDto>> UpdateProfile([FromBody] CarRentalProviderProfileUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var updatedProfile = await _carRentalProviderService.UpdateCarRentalProviderProfileAsync(userId, model);
            if (updatedProfile == null)
            {
                return BadRequest("Failed to update car rental provider profile. Check if profile exists or data is valid.");
            }
            return Ok(updatedProfile);
        }

        // Add other Car Rental Provider specific actions here (e.g., managing vehicles, rental agreements)
    }
}