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
        public async Task<ActionResult> RegisterHotelProvider(
            [FromForm] HotelProviderRegistrationRequestDto model,
            [FromForm] List<IFormFile> files)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fileValidationResult = ValidateFiles(files, 2, 2 * 1024 * 1024); // 2 files, 2MB each
            if (!fileValidationResult.Success)
            {
                return BadRequest(new AuthResultDto { Errors = fileValidationResult.Errors });
            }


            var result = await _authService.RegisterHotelProviderAsync(model, files);
            return HandleAuthResult(result);
        }


        [HttpPost("register-hospital-provider")]
        public async Task<ActionResult> RegisterHospitalProvider(
            [FromForm] HospitalProviderRegistrationRequestDto model
            , [FromForm] List<IFormFile> files)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

             

            var fileValidationResult = ValidateFiles(files, 2, 2 * 1024 * 1024); // 2 files, 2MB each
            if (!fileValidationResult.Success)
            {
                return BadRequest(new AuthResultDto { Errors = fileValidationResult.Errors });
            }

            var result = await _authService.RegisterHospitalProviderAsync(model,files);
            return HandleAuthResult(result);
        }


        [HttpPost("register-car-rental-provider")]
        public async Task<ActionResult> RegisterCarRentalProvider(
            [FromForm] CarRentalProviderRegistrationRequestDto model,
            [FromForm] List<IFormFile> files)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var fileValidationResult = ValidateFiles(files, 2, 2 * 1024 * 1024); // 2 files, 2MB each
            if (!fileValidationResult.Success)
            {
                return BadRequest(new AuthResultDto { Errors = fileValidationResult.Errors });
            }

            var result = await _authService.RegisterCarRentalProviderAsync(model,files);
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


        [HttpGet("confirm-new-email")]
        public async Task<IActionResult> ConfirmNewEmail(
        [FromQuery] string userId,
        [FromQuery] string newEmail, // <--- IMPORTANT: New email from the URL
        [FromQuery] string token)
            {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newEmail) || string.IsNullOrEmpty(token))
            {
                return BadRequest(new AuthResultDto { Success = false, Errors = new[] { "Invalid confirmation link provided (missing parameters)." } });
            }

            var result = await _authService.ConfirmNewEmailAsync(userId, newEmail, token);

            if (result.Success)
            {
                return Ok(result); // Or Redirect to a success page
            }
            else
            {
                return BadRequest(result); // Or Redirect to an error page
            }
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