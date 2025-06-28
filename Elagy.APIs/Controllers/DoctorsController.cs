// Elagy.APIs/Controllers/DoctorsController.cs
using Elagy.Core.DTOs.Doctor;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Enums; // For Status enum
using Elagy.Core.IServices; // For IDoctorService
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // For StatusCodes
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Base route: /api/Doctors
    public class DoctorsController : BaseApiController // Inherits from your BaseApiController
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        // --- PUBLIC/WEBSITE ENDPOINTS ---

        [HttpGet("hospital-specialty/{hospitalSpecialtyId}")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetDoctorsByHospitalSpecialty(
            int hospitalSpecialtyId,
            [FromQuery] int PageNumber = 1,
            [FromQuery] int PageSize = 10,
            [FromQuery] string? SearchTerm = null)
        {
            if (hospitalSpecialtyId <= 0 || PageNumber < 1 || PageSize < 1)
            {
                return BadRequest("Invalid pagination parameters or HospitalSpecialtyId.");
            }
            try
            {
                var paginationParams = new PaginationParameters { PageNumber = PageNumber, PageSize = PageSize, SearchTerm = SearchTerm };
                var result = await _doctorService.GetAllDoctorsPerHospitalSpecialty(hospitalSpecialtyId, paginationParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting doctors for hospital specialty ID: {hospitalSpecialtyId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving doctors for the specialty.");
            }
        }

        // --- HOSPITAL ADMIN DASHBOARD ENDPOINTS ---

        [HttpGet("my-hospital")]
        [Authorize(Roles = "HospitalServiceProvider")] 
        public async Task<IActionResult> GetMyHospitalDoctors(
            [FromQuery] int PageNumber = 1,
            [FromQuery] int PageSize = 10,
            [FromQuery] string? SearchTerm = null,
            [FromQuery] Status? UserStatus = null)
        {
            var hospitalId = GetCurrentUserId(); // Get hospital ID from the authenticated user's token
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from your token.");

            if (PageNumber < 1 || PageSize < 1) return BadRequest("PageNumber and PageSize must be greater than 0.");

            try
            {
                var paginationParams = new PaginationParameters { PageNumber = PageNumber, PageSize = PageSize, SearchTerm = SearchTerm, UserStatus = UserStatus };
                var result = await _doctorService.GetAllDoctorsForAdminDashboardAsync(hospitalId, paginationParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting doctors for hospital admin dashboard for Hospital ID: {hospitalId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving doctors for your hospital.");
            }
        }

        // POST: api/Doctors
        [HttpPost]
        [Authorize(Roles = "HospitalServiceProvider")]
        public async Task<IActionResult> CreateDoctor([FromBody] DoctorCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var hospitalId = GetCurrentUserId(); // Get hospital ID from the authenticated user's token
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from your token.");

            try
            {
                var result = await _doctorService.CreateDoctorAsync(createDto, hospitalId);
     
                return CreatedAtAction(nameof(GetDoctorById), new { doctorId = result.Id }, result);
            }
            catch (ArgumentException ex) // For invalid HospitalSpecialtyId, GovernorateId, CountryId etc.
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // For email already exists, inactive specialty, Identity creation failures etc.
            {
                return Conflict(ex.Message); // 409 Conflict for resource conflicts or business rule violations
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating doctor account.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while creating the doctor account.");
            }
        }

        // GET: api/Doctors/{doctorId}

        [HttpGet("{doctorId}")]
        [Authorize(Roles = "HospitalServiceProvider")] 
        public async Task<IActionResult> GetDoctorById(string doctorId)
        {
            if (string.IsNullOrWhiteSpace(doctorId)) return BadRequest("Doctor ID cannot be empty.");
            try
            {
                var result = await _doctorService.GetDoctorByIdAsync(doctorId);
                if (result == null) return NotFound("Doctor not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting doctor by ID: {doctorId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the doctor.");
            }
        }


        [HttpPut("{doctorId}")]
        [Authorize(Roles = "HospitalServiceProvider")]
        public async Task<IActionResult> UpdateDoctor(string doctorId, [FromBody] DoctorUpdateDto updateDto)
        {
            // Validate route ID matches DTO ID (if DTO had ID) or simply ensures route ID is valid
            if (string.IsNullOrWhiteSpace(doctorId)) return BadRequest("Doctor ID cannot be empty.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var hospitalId = GetCurrentUserId(); // Get hospital ID from the authenticated user's token
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from your token.");

            try
            {
                // Call service method with doctorId from route, DTO from body, and hospitalId from token
                var result = await _doctorService.UpdateDoctorAsync(doctorId, updateDto, hospitalId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) // Doctor not found
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex) // Invalid HospitalSpecialtyId, GovernorateId, CountryId etc.
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex) // Doctor not affiliated with hospital
            {
                return Forbid(ex.Message); // 403 Forbidden
            }
            catch (InvalidOperationException ex) // For email conflict, inactive specialty, Identity update failures etc.
            {
                return Conflict(ex.Message); // 409 Conflict
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating doctor with ID: {doctorId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while updating the doctor.");
            }
        }

        
        [HttpDelete("Deactivate/{doctorId}")]
        [Authorize(Roles = "HospitalServiceProvider")]
        public async Task<IActionResult> DeleteDoctor(string doctorId)
        {
            if (string.IsNullOrWhiteSpace(doctorId)) return BadRequest("Doctor ID cannot be empty.");

            var hospitalId = GetCurrentUserId(); // Get hospital ID from the authenticated user's token
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from your token.");

            try
            {
                
                var result = await _doctorService.DeleteDoctorAsync(doctorId, hospitalId);
                return Ok(result); 
            }
            catch (KeyNotFoundException ex) 
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message); // 403 Forbidden
            }
            catch (InvalidOperationException ex) // Identity deactivation failure
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message); // Or 409 Conflict if specific reason
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting doctor with ID: {doctorId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while deleting the doctor.");
            }
        }


        [HttpPut("activate/{doctorId}")]
        [Authorize(Roles = "HospitalServiceProvider")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DoctorProfileDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // If doctor not affiliated
        [ProducesResponseType(StatusCodes.Status404NotFound)] // If doctor not found
        [ProducesResponseType(StatusCodes.Status409Conflict)] // If already active or other business conflicts
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActivateDoctor(string doctorId)
        {
            if (string.IsNullOrWhiteSpace(doctorId)) return BadRequest("Doctor ID cannot be empty.");

            var hospitalId = GetCurrentUserId(); // Get hospital ID from token
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from your token.");

            try
            {
                // 1. Get the current doctor's full profile. This is crucial
                // because DoctorUpdateDto has [Required] fields that must be sent,
                // even if only Status is changing.
                var currentDoctorProfile = await _doctorService.GetDoctorByIdAsync(doctorId);
                if (currentDoctorProfile == null) return NotFound("Doctor not found.");

                // 2. Validate doctor's affiliation *before* attempting update.
                if (currentDoctorProfile.HospitalId != hospitalId)
                {
                    return Forbid("You do not have permission to activate this doctor as they are not affiliated with your hospital.");
                }

                // 3. Create a DoctorUpdateDto based on current data,
    
                var updateDto = new DoctorUpdateDto
                {
                   
                    FirstName = currentDoctorProfile.FirstName,
                    LastName = currentDoctorProfile.LastName,
                    Email = currentDoctorProfile.Email, 
                    PhoneNumber = currentDoctorProfile.Phone,
                    Gender = currentDoctorProfile.Gender,
                    Address = currentDoctorProfile.Address,
                    City = currentDoctorProfile.City,
                    GovernorateId = currentDoctorProfile.GovernorateId, 
                    CountryId = currentDoctorProfile.CountryId,
                    DateOfBirth = currentDoctorProfile.DateOfBirth,
                    MedicalLicenseNumber = currentDoctorProfile.MedicalLicenseNumber,
                    YearsOfExperience = currentDoctorProfile.YearsOfExperience,
                    Bio = currentDoctorProfile.Bio,
                    Qualification = currentDoctorProfile.Qualification,
                    HospitalSpecialtyId = currentDoctorProfile.SpecialtyId,
                    Status = Status.Active // Set the desired status
                };

                // 4. Call the main UpdateDoctorAsync service method
                var result = await _doctorService.UpdateDoctorAsync(doctorId, updateDto, hospitalId);

                return Ok(result); // Return the updated DoctorProfileDto
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex) // For business rule violations (e.g., already active)
            {
                return Conflict(ex.Message); // 409 Conflict
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error activating doctor {doctorId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while activating the doctor.");
            }
        }
    }
}