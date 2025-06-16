// Elagy.APIs/Controllers/DoctorsController.cs


using Elagy.Core.DTOs.DoctorDTO;
using Elagy.Core.DTOs.SpecialtyDTO;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Elagy.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "HospitalAdmin")] // Only hospital admins can manage doctors
    public class DoctorsController : BaseApiController
    {
        private readonly IDoctorService _doctorService;

        public DoctorsController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // GET: api/Doctors (Admin dashboard list of doctors)
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DoctorTableDto>>> GetAllDoctorsForAdmin()
        {
            var hospitalId = GetCurrentUserId(); // Get the authenticated admin's HospitalAsset.Id
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from token.");

            // Pass hospitalId to service for scoping
            var doctors = await _doctorService.GetAllDoctorsForAdminDashboardAsync(hospitalId);
            return Ok(doctors);
        }

        // GET: api/Doctors/{id} (Admin view single doctor details modal)
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Add Forbidden for resource mismatch
        public async Task<ActionResult<DoctorTableDto>> GetDoctorForAdmin(string id) 
        {
            var hospitalId = GetCurrentUserId(); // Get the authenticated admin's HospitalAsset.Id
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from token.");

            // Pass hospitalId to service for scoping
            var doctor = await _doctorService.GetDoctorByIdForAdminAsync(id, hospitalId);
            if (doctor == null)
            {
                return NotFound("Doctor not found or you do not have permission to view this doctor.");
            }
            return Ok(doctor);
        }

        // POST: api/Doctors (Admin create doctor)
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DoctorTableDto>> CreateDoctor([FromBody] DoctorCreateDto createDto) // Changed return type to DoctorDto
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hospitalId = GetCurrentUserId(); // Get the authenticated admin's HospitalAsset.Id
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from token.");

            try
            {
                // Pass hospitalId to service for scoping
                var doctorDto = await _doctorService.CreateDoctorAsync(createDto, hospitalId);
                // Return DoctorDto here, which includes the generated Id
                return CreatedAtAction(nameof(GetDoctorForAdmin), new { id = doctorDto.Id }, doctorDto);
            }
            catch (ArgumentException ex) // For invalid HospitalSpecialtyId or duplicate email
            {
                return BadRequest(ex.Message);
            }
            catch (ApplicationException ex) // For Identity errors
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        // This handles updating doctor details and REASSIGNING them to a different specialty.
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Add Forbidden for resource mismatch
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDoctor(string id, [FromBody] DoctorUpdateDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("ID mismatch.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hospitalId = GetCurrentUserId(); // Get the authenticated admin's HospitalAsset.Id
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from token.");

            try
            {
                // Pass hospitalId to service for scoping
                var updated = await _doctorService.UpdateDoctorAsync(updateDto, hospitalId);
                if (!updated)
                {
                    // Service returns false if doctor not found. If service throws Unauthorized, it's a Forbid.
                    return NotFound("Doctor not found.");
                }
                return NoContent();
            }
            catch (ArgumentException ex) // For invalid HospitalSpecialtyId or other validation issues
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException) // From service if doctor does not belong to this hospital
            {
                return Forbid("You do not have permission to update this doctor."); // 403 Forbidden
            }
            catch (ApplicationException ex) // For Identity update errors
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELETE: api/Doctors/{id} (Admin delete doctor)
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Added for clarity
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var hospitalId = GetCurrentUserId(); // Get the authenticated admin's HospitalAsset.Id
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from token.");

            try
            {
                // Calls DoctorService.DeleteDoctorAsync(id, hospitalId)
                var deleted = await _doctorService.DeleteDoctorAsync(id, hospitalId);
                if (!deleted)
                {
                    return NotFound("Doctor not found."); // If service threw Unauthorized, it would be caught below
                }
                return NoContent(); // Success, 204 No Content
            }
            catch (UnauthorizedAccessException) // Catches the exception from the service if not permitted
            {
                return Forbid("You do not have permission to delete this doctor."); // 403 Forbidden
            }
            catch (ApplicationException ex) // Catches Identity-related errors
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        // GET: api/Doctors/hospital-specialties/{hospitalId}
        // This endpoint helps admins get the list of HospitalSpecialties for a dropdown when adding/updating doctors
        [HttpGet("hospital-specialties")] // No {hospitalId} in route, get from token
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // If hospitalId is invalid/not found
        public async Task<ActionResult<IEnumerable<HospitalSpecialtyDto>>> GetHospitalSpecialtiesForDoctorAssignment()
        {
            var hospitalId = GetCurrentUserId(); // Get the authenticated admin's HospitalAsset.Id
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from token.");

            var hospitalSpecialties = await _doctorService.GetAvailableHospitalSpecialtiesForAssignmentAsync(hospitalId);
            if (hospitalSpecialties == null || !hospitalSpecialties.Any())
            {
                return NotFound("No specialties are assigned to your hospital."); // More specific error
            }
            return Ok(hospitalSpecialties);
        }
    }
}