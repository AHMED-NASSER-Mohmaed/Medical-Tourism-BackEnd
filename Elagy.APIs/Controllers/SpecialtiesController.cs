// Elagy.APIs/Controllers/SpecialtiesController.cs

using Elagy.Core.DTOs;
using Elagy.Core.DTOs.Specialty;
using Elagy.Core.IServices.ISpecialtyService;
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


        [HttpGet]
        [Authorize(Roles = "SuperAdmin, HospitalAdmin")]
        public async Task<ActionResult<IEnumerable<SpecialtyDto>>> GetAllSpecialtiesForAdmin()
        {
            var userId = GetCurrentUserId();
            var isSuperAdmin = User.IsInRole("SuperAdmin");

            IEnumerable<SpecialtyDto> specialties;

            if (isSuperAdmin)
            {
                specialties = await _specialtyService.GetAllSpecialtiesAsync(); // Get all global specialties
            }
            else // HospitalAdmin
            {
                // Get specialties linked to this hospital
                specialties = await _specialtyService.GetSpecialtiesForHospitalAdminDashboardAsync(userId);
                if (specialties == null || !specialties.Any())
                {
                    return NotFound("No specialties found for your hospital.");
                }
            }
            return Ok(specialties);
        }

        // This endpoint provides the list of global specialties that the current hospital admin CAN link to their hospital.
        [HttpGet("available-to-link")]
        [Authorize(Roles = "HospitalAdmin")]
        public async Task<ActionResult<IEnumerable<SpecialtyDto>>> GetAvailableSpecialtiesToLink()
        {
            var hospitalId = GetCurrentUserId();
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from token.");

            var availableSpecialties = await _specialtyService.GetAvailableGlobalSpecialtiesToLinkAsync(hospitalId);
            return Ok(availableSpecialties);
        }


        [HttpGet("{id}")]
        [Authorize(Roles = "SuperAdmin, HospitalAdmin")]
        public async Task<ActionResult<SpecialtyDto>> GetSpecialtyForAdmin(int id)
        {
            var specialty = await _specialtyService.GetSpecialtyByIdAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }

            var userId = GetCurrentUserId();
            if (User.IsInRole("HospitalAdmin"))
            {
                var isAssociated = await _specialtyService.IsSpecialtyAssociatedWithHospitalAsync(id, userId);
                if (!isAssociated)
                {
                    return Forbid("You do not have permission to view details for this specialty as it's not associated with your hospital.");
                }
            }
            return Ok(specialty);
        }


        [HttpPost]
        [Authorize(Roles = "SuperAdmin")] // ONLY SuperAdmin can create global specialties
        public async Task<ActionResult<SpecialtyDto>> CreateSpecialtyGlobal([FromBody] SpecialtyCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var specialtyDto = await _specialtyService.CreateSpecialtyAsync(createDto);
                return CreatedAtAction(nameof(GetSpecialtyForAdmin), new { id = specialtyDto.Id }, specialtyDto);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message); // 409 Conflict if name already exists
            }
        }

        // POST: api/Specialties/link-to-hospital (HospitalAdmin links existing specialty to their hospital)
        // This is the "Add Clinic" action from the Hospital Admin's perspective.
        [HttpPost("link-to-hospital")]
        [Authorize(Roles = "HospitalAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Success, resource linked
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Invalid SpecialtyId or data
        [ProducesResponseType(StatusCodes.Status409Conflict)] // Specialty already linked to hospital
        public async Task<IActionResult> LinkSpecialtyToHospital([FromBody] SpecialtyLinkToHospitalDto linkDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hospitalId = GetCurrentUserId(); // Get the authenticated admin's HospitalAsset.Id
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from token.");

            try
            {
                var linked = await _specialtyService.LinkSpecialtyToHospitalAsync(linkDto, hospitalId);
                if (linked)
                {
                    return Ok("Specialty successfully linked to your hospital.");
                }
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to link specialty for an unknown reason.");
            }
            catch (ArgumentException ex) // Invalid SpecialtyId or HospitalId
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex) // Already linked
            {
                return Conflict(ex.Message);
            }
        }

        // PUT: api/Specialties/{id} (SuperAdmin updates a global specialty)
        [HttpPut("{id}")]
        [Authorize(Roles = "SuperAdmin")] // ONLY SuperAdmin can update global specialties
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)] // If name conflict
        public async Task<IActionResult> UpdateSpecialtyGlobal(int id, [FromBody] SpecialtyUpdateDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("ID mismatch.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updated = await _specialtyService.UpdateSpecialtyAsync(updateDto);
                if (!updated)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // DELETE: api/Specialties/{id} (SuperAdmin deletes a global specialty)
        [HttpDelete("{id}")]
        [Authorize(Roles = "SuperAdmin")] // ONLY SuperAdmin can delete global specialties
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteSpecialtyGlobal(int id)
        {
            try
            {
                var deleted = await _specialtyService.DeleteSpecialtyAsync(id);
                if (!deleted)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // Conflict if associated with hospitals
            }
        }

        // DELETE: api/Specialties/{specialtyId}/from-hospital (HospitalAdmin removes specialty from their hospital)
        // This is the "Delete Clinic" action from the Hospital Admin's perspective.
        [HttpDelete("{specialtyId}/from-hospital")]
        [Authorize(Roles = "HospitalAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)] // If doctors are linked
        public async Task<IActionResult> RemoveSpecialtyFromHospital(int specialtyId)
        {
            var hospitalId = GetCurrentUserId();
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from token.");

            try
            {
                var removed = await _specialtyService.RemoveSpecialtyFromHospitalAsync(specialtyId, hospitalId);
                if (!removed)
                {
                    return NotFound("Specialty not found or not associated with your hospital.");
                }
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // Conflict if doctors are still assigned
            }
        }
    }
}