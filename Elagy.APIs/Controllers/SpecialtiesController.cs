// Elagy.APIs/Controllers/SpecialtiesController.cs

using Elagy.Core.DTOs.Pagination; // Needed for PagedResponseDto
using Elagy.Core.DTOs.Specialty;
using Elagy.Core.Enums; // Needed for Status enum
using Elagy.Core.IServices.ISpecialtyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // For StatusCodes
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Base route: /api/Specialties
    public class SpecialtiesController : BaseApiController // Assumes BaseApiController is defined
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtiesController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        // --- GET Endpoints ---

        [HttpGet] // MODIFIED: Added FromQuery parameters
        [Authorize(Roles = "SuperAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponseDto<SpecialtyResponseDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllSpecialtiesForSuperAdminDashboard(
             [FromQuery] int PageNumber = 1,
             [FromQuery] int PageSize = 10,
             [FromQuery] string? SearchTerm = null,
             [FromQuery] Status? UserStatus = null) // MODIFIED: Added query parameters
        {
            if (PageNumber < 1 || PageSize < 1)
            {
                return BadRequest("PageNumber and PageSize must be greater than 0.");
            }

            try
            {
                var paginationParameters = new PaginationParameters
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm,
                    UserStatus = UserStatus // Maps to Specialty.IsActive
                };

                var result = await _specialtyService.GetAllSpecialties(paginationParameters); 
                return Ok(result);
            }
            catch (Exception ex)
            {
             
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("available-for-linking/my-hospital")] // Changed route to directly use current user's hospital
        [Authorize(Roles = "HospitalServiceProvider")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<SpecialtyResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Added for GetCurrentUserId failure
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableSpecialtiesToLinkToMyHospital() 
        {
            var hospitalId = GetCurrentUserId();
            if (hospitalId == null)
            {
                return Unauthorized("Hospital ID could not be determined from your token.");
            }
            try
            {
                var result = await _specialtyService.GetAvailableGlobalSpecialtiesToLinkAsync(hospitalId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("my-hospital")] 
        [Authorize(Roles = "HospitalServiceProvider")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedResponseDto<SpecialtyResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Added for GetCurrentUserId failure
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyHospitalSpecialties() // Renamed to reflect 'my-hospital'
        {
            var hospitalId = GetCurrentUserId(); // <<< Using GetCurrentUserId() here
            if (hospitalId == null)
            {
                return Unauthorized("Hospital ID could not be determined from your token.");
            }
            try
            {
                var result = await _specialtyService.GetAllSpecialtiesForHospital(hospitalId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        // GET: api/Specialties/{id}
        /// <summary>
        /// Retrieves a single specialty by its ID (globally active).
        /// </summary>
        /// <param name="id">The ID of the specialty.</param>
        /// <returns>The specialty response DTO.</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin, HospitalServiceProvider")] // Both roles can view specific specialties
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpecialtyResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSpecialtyById(int id)
        {
            try
            {
                // Your service method GetSpecialtyIdAsync implicitly filters by IsActive=true.
                var result = await _specialtyService.GetSpecialtyIdAsync(id); // Using the specific GetSpecialtyIdAsync from your repo interface
                if (result == null)
                {
                    return NotFound($"Specialty with ID {id} not found or is inactive globally.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        // --- POST Endpoints ---

        // POST: api/Specialties
        /// <summary>
        /// Creates a new global specialty.
        /// </summary>
        /// <param name="createDto">Specialty creation data.</param>
        /// <returns>The created specialty response DTO.</returns>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")] // Only SuperAdmins can create global specialties
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SpecialtyResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateSpecialty([FromBody] SpecialtyCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _specialtyService.CreateSpecialty(createDto);
                return CreatedAtAction(nameof(GetSpecialtyById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        // POST: api/Specialties/{specialtyId}/link-to-my-hospital
        /// <summary>
        /// Links an active global specialty to the current authenticated hospital.
        /// </summary>
        /// <param name="specialtyId">The ID of the specialty to link.</param>
        /// <returns>The SpecialtyLinkToHospitalDto representing the new/reactivated link.</returns>
        [HttpPost("link-to-my-hospital/{specialtyId}")] // Changed route
        [Authorize(Roles = "HospitalServiceProvider")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpecialtyLinkToHospitalDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Added for GetCurrentUserId failure
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LinkSpecialtyToMyHospital(int specialtyId) // Removed hospitalId from parameter
        {
            var hospitalId = GetCurrentUserId(); 
            if (hospitalId == null)
            {
                return Unauthorized("Hospital ID could not be determined from your token.");
            }
            try
            {
                var result = await _specialtyService.LinkSpecialtyToHospital(specialtyId, hospitalId);
                if (result == null)
                {
                    return NotFound("Specialty or Hospital not found.");
                }
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        // --- PUT Endpoints ---

        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")] // Only SuperAdmins can update global specialties
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpecialtyResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateSpecialty(int id, [FromBody] SpecialtyUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _specialtyService.UpdateSpecialty(id, updateDto); // Pass route ID
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        // PUT: api/Specialties/my-hospital/status
        [HttpPut("my-hospital/status")] // Changed route
        [Authorize(Roles = "HospitalServiceProvider")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpecialtyLinkToHospitalDto))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Added for GetCurrentUserId failure
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeSpecialtiesStatusForMyHospital([FromBody] Status status) // Removed hospitalId from parameter
        {
            var hospitalId = GetCurrentUserId(); // <<< Using GetCurrentUserId() here
            if (hospitalId == null)
            {
                return Unauthorized("Hospital ID could not be determined from your token.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _specialtyService.CahngeSTSpecialtiesForHospitalAdminDashboard(hospitalId, status);
                return Ok(result); // Returns the placeholder DTO from the service
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        // --- DELETE Endpoints ---
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")] // Only SuperAdmins can delete global specialties
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpecialtyResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)] // If associated with hospitals
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteSpecialty(int id)
        {
            try
            {
                var result = await _specialtyService.DeleteSpecialtyAsync(id);
                if (result == null) // Service returns null if not found or already inactive
                {
                    return NotFound($"Specialty with ID {id} not found or is already inactive.");
                }
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}