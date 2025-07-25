﻿

using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Specialty;
using Elagy.Core.Enums; // Needed for Status enum
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elagy.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class SpecialtiesController : BaseApiController
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtiesController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

       

        [HttpGet("SuperAdmin")]
        [AllowAnonymous]
        
        public async Task<IActionResult> GetAllSpecialtiesForSuperAdminDashboard(
             [FromQuery] int PageNumber = 1,
             [FromQuery] int PageSize = 10,
             [FromQuery] string? SearchTerm = null,
             [FromQuery] Status? UserStatus = null,
             [FromQuery] int? specialtyId = null)
             
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
                    UserStatus = UserStatus,
                    SpecialtyId=specialtyId
                    
                };

                var result = await _specialtyService.GetAllSpecialties(paginationParameters); 
                return Ok(result);
            }
            catch (Exception ex)
            {
             
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("Website")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllSpecialtiesForWebsite(
            [FromQuery] int PageNumber = 1,
            [FromQuery] int PageSize = 10,
            [FromQuery] string? SearchTerm = null,
            [FromQuery] Status UserStatus = Status.Active,
            [FromQuery] int? specialtyId = null)

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
                    UserStatus = UserStatus,
                    SpecialtyId = specialtyId

                };

                var result = await _specialtyService.GetAllSpecialties(paginationParameters);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("available-for-linking/my-hospital")] 
        [Authorize(Roles = "HospitalServiceProvider")]
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

        [HttpGet("HospitalAdmin")] 
        [Authorize(Roles = "HospitalServiceProvider")]
        public async Task<IActionResult> GetMyHospitalSpecialties([FromQuery] int PageNumber = 1,
             [FromQuery] int PageSize = 10,
             [FromQuery] string? SearchTerm = null,
             [FromQuery] bool? ISActive = null,
             [FromQuery] int? specialtyId = null) 
        {
            if (PageNumber < 1 || PageSize < 1)
            {
                return BadRequest("PageNumber and PageSize must be greater than 0.");
            }

            var hospitalId = GetCurrentUserId(); 
            if (hospitalId == null)
            {
                return Unauthorized("Hospital ID could not be determined from your token.");
            }
            try
            {
                var paginationParameters = new PaginationParameters
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm,
                    FilterIsActive = ISActive,
                    SpecialtyId = specialtyId


                };
                var result = await _specialtyService.GetHospitalSpecialtiesWithLinks(hospitalId, paginationParameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        // GET: api/Specialties/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin, HospitalServiceProvider")] 

        public async Task<IActionResult> GetSpecialtyById(int id)
        {
            try
            {
                
                var result = await _specialtyService.GetSpecialtyIdAsync(id); 
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

        [HttpPost("SuperAdmin")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateSpecialty([FromForm] SpecialtyCreateDto createDto, IFormFile? SpecialtyImageFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _specialtyService.CreateSpecialty(createDto, SpecialtyImageFile);
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




        [HttpPost("link-to-hospital-admin/{specialtyId}")]
        [Authorize(Roles = "HospitalServiceProvider")]

        public async Task<IActionResult> LinkSpecialtyToMyHospital(int specialtyId) 
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

        [HttpPut("SuperAdmin/{id}")]
        [Authorize(Roles = "SuperAdmin")] 
        public async Task<IActionResult> UpdateSpecialty(int id, [FromForm] SpecialtyUpdateDto updateDto, IFormFile? newspecialtyImageFile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _specialtyService.UpdateSpecialty(id, updateDto, newspecialtyImageFile); // Pass route ID
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

        [HttpPut("myhospital/status-link/{specialtyId}")]
        [Authorize(Roles = "HospitalServiceProvider")]
        public async Task<IActionResult> ChangeSpecificHospitalSpecialtyLinkStatus(
     int specialtyId,
     [FromBody] Status status)
        {
            var hospitalId = GetCurrentUserId();
            if (hospitalId == null)
                return Unauthorized("Hospital ID not found in token");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _specialtyService.ChangeSpecificHospitalSpecialtyStatusAsync(
                    hospitalId, specialtyId, status);

                if (result == null)
                    return NotFound($"Specialty link {specialtyId} not found or not associated with your hospital");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Internal server error: {ex.Message}");
            }
        }

        // --- DELETE Endpoints ---
        [HttpDelete("SuperAdmin{id}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteSpecialty(int id)
        {
            try
            {
                var result = await _specialtyService.DeleteSpecialtyAsync(id);
                if (result == null)
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


        [HttpPut("activate/{id}")]
        [Authorize(Roles = "SuperAdmin")] 

        public async Task<IActionResult> ActivateSpecialty(int id)
        {
            try
            {
           
                var result = await _specialtyService.ChangeSpecialtyStatusAsync(id, true);

                if (result == null)
                {
                    return NotFound($"Specialty with ID {id} not found.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
            
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred while activating the specialty: {ex.Message}");
            }
        }
    }
}