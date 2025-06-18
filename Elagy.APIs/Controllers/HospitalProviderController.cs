using Elagy.Core.DTOs.User;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;  
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    // Protect this controller, allowing only ServiceProviders that are Hospital type
    [Authorize(Roles = "HospitalServiceProvider")] // Assuming a generic ServiceProvider role for all providers
    public class HospitalProviderController : ProfileImageBaseController
    {
        private readonly IHospitalProviderService _hospitalProviderService;

        public HospitalProviderController(
            IImageProfile profileImageService,            
            IHospitalProviderService hospitalProviderService,  
            ILogger<HospitalProviderController> logger)   
            : base(profileImageService, logger)                                       
        {
            _hospitalProviderService = hospitalProviderService;
        }


        [HttpGet("profile")]
        public async Task<ActionResult<HospitalProviderProfileDto>> GetProfile()
        {
            var userId = GetCurrentUserId(); // Get current user's ID from token
            if (userId == null) return Unauthorized();

            var profile = await _hospitalProviderService.GetHospitalProviderProfileAsync(userId);
            if (profile == null)
            {
                return NotFound("Hospital provider profile not found or user is not a Hospital provider.");
            }
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<HospitalProviderProfileDto>> UpdateProfile([FromBody] HospitalProviderProfileUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var updatedProfile = await _hospitalProviderService.UpdateHospitalProviderProfileAsync(userId, model);
            if (updatedProfile == null)
            {
                return BadRequest("Failed to update hospital provider profile. Check if profile exists or data is valid.");
            }
            return Ok(updatedProfile);
        }

        // Add other Hospital Provider specific actions here (e.g., managing departments, appointments)
    }
}