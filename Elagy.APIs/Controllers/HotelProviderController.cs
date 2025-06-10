using Elagy.Core.DTOs.User;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; // For [Authorize] attribute
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    // Protect this controller, allowing only ServiceProviders that are Hotel type
    [Authorize(Roles = "ServiceProvider")] // Assuming a generic ServiceProvider role for all providers
    public class HotelProviderController : BaseApiController
    {
        private readonly IHotelProviderService _hotelProviderService;

        public HotelProviderController(IHotelProviderService hotelProviderService)
        {
            _hotelProviderService = hotelProviderService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<HotelProviderProfileDto>> GetProfile()
        {
            var userId = GetCurrentUserId(); // Get current user's ID from token
            if (userId == null) return Unauthorized();

            var profile = await _hotelProviderService.GetHotelProviderProfileAsync(userId);
            if (profile == null)
            {
                // Note: If profile is null, it could mean not found, or not the correct asset type for this controller.
                // You might want more specific error handling.
                return NotFound("Hotel provider profile not found or user is not a Hotel provider.");
            }
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<HotelProviderProfileDto>> UpdateProfile([FromBody] HotelProviderProfileUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var updatedProfile = await _hotelProviderService.UpdateHotelProviderProfileAsync(userId, model);
            if (updatedProfile == null)
            {
                return BadRequest("Failed to update hotel provider profile. Check if profile exists or data is valid.");
            }
            return Ok(updatedProfile);
        }

        // Add other Hotel Provider specific actions here (e.g., managing rooms, bookings, availability)
    }
}