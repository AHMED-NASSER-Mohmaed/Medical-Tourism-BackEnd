using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    // Inherit from BaseApiController for common functionality
    [Route("api/[controller]")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // --- Registration Endpoints ---
        [HttpPost("register-patient")]
        public async Task<ActionResult> RegisterPatient([FromBody] PatientRegistrationRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterPatientAsync(model);
            return HandleAuthResult(result);
        }

        [HttpPost("register-hotel-provider")]
        public async Task<ActionResult> RegisterHotelProvider([FromBody] HotelProviderRegistrationRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterHotelProviderAsync(model);
            return HandleAuthResult(result);
        }

        [HttpPost("register-hospital-provider")]
        public async Task<ActionResult> RegisterHospitalProvider([FromBody] HospitalProviderRegistrationRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterHospitalProviderAsync(model);
            return HandleAuthResult(result);
        }

        [HttpPost("register-car-rental-provider")]
        public async Task<ActionResult> RegisterCarRentalProvider([FromBody] CarRentalProviderRegistrationRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.RegisterCarRentalProviderAsync(model);
            return HandleAuthResult(result);
        }

        // --- Login Endpoint ---
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.LoginAsync(model);
            return HandleAuthResult(result);
        }

        // --- Email Confirmation Endpoint ---
        [HttpGet("confirm-email")]
        public async Task<ActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            var result = await _authService.ConfirmEmailAsync(userId, token);
            // You might redirect to a success/failure page here in a real app
            return HandleAuthResult(result);
        }

        // --- Password Management Endpoints ---
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto model) // You'll need to define ForgotPasswordRequestDto
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ForgotPasswordAsync(model.Email);
            // Always return a generic success message to prevent user enumeration
            return Ok(new AuthResultDto { Success = true, Message = "If an account with that email exists, a password reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _authService.ResetPasswordAsync(model);
            return HandleAuthResult(result);
        }

        [HttpPut("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize] // Requires user to be logged in
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId(); // Get current user's ID from token
            if (userId == null) return Unauthorized();

            var result = await _authService.ChangePasswordAsync(userId, model);
            return HandleAuthResult(result);
        }

        [HttpPut("change-email")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<ActionResult> ChangeEmail([FromBody] ChangeEmailRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var result = await _authService.ChangeEmailAsync(userId, model);
            return HandleAuthResult(result);
        }
    }
}