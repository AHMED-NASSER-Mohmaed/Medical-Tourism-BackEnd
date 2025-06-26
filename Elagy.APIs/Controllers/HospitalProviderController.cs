using Elagy.Core.DTOs.DoctorSchedule;
using Elagy.Core.DTOs.User;
using Elagy.Core.IServices;
using Elagy.Core.IServices.ISpecialtyService;
using Microsoft.AspNetCore.Authorization;  
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    // Protect this controller, allowing only ServiceProviders that are Hospital type
    [Authorize(Roles = "HospitalServiceProvider")] // Assuming a generic ServiceProvider role for all providers
    public class HospitalProviderController : ProfileImageBaseController
    {
        private readonly IHospitalProviderService _hospitalProviderService;
        private readonly IScheduleService _scheduleService;

        public HospitalProviderController(
            IImageProfile profileImageService,
             IScheduleService scheduleService,
            IHospitalProviderService hospitalProviderService,  
            ILogger<HospitalProviderController> logger)   
            : base(profileImageService, logger)                                       
        {
            _hospitalProviderService = hospitalProviderService;
            _scheduleService = scheduleService;
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



        //-----------assign schedules--------

        [HttpPost("Assign_Schedule")]
        public async Task<ActionResult<ScheduleDto>> CreateSchedule([FromBody] CreateScheduleDto createDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation errors during schedule creation: {Errors}", ModelState);
                return BadRequest(ModelState);
            }

            var hospitalAdminId = GetCurrentUserId();
            if (string.IsNullOrEmpty(hospitalAdminId))
            {
                _logger.LogWarning("Unauthorized attempt to create schedule: Hospital ID could not be determined from token.");
                return Unauthorized("Hospital ID could not be determined from token.");
            }

            try
            {
                var createdSchedule = await _scheduleService.AssignScheduleByAdminAsync(createDto, hospitalAdminId);
                _logger.LogInformation("Admin {HospitalId} successfully created schedule {ScheduleId} for doctor {DoctorId}.", hospitalAdminId, createdSchedule.Id, createdSchedule.DoctorId);
                return CreatedAtAction(
                    nameof(GetScheduleById), // Standard practice: return 201 Created with a link to the new resource
                    new { id = createdSchedule.Id },
                    createdSchedule
                );
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request during schedule creation by admin {HospitalId}: {Message}", hospitalAdminId, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Admin {HospitalId} attempted to create a schedule for an unauthorized doctor/hospital specialty.", hospitalAdminId);
                return Forbid("You do not have permission to assign schedules for this doctor or hospital specialty.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during schedule creation by admin {HospitalId}.", hospitalAdminId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the schedule.");
            }
        }

        [HttpGet("ShowAllSchedules")]
        public async Task<ActionResult<IEnumerable<ScheduleDto>>> GetHospitalSchedules()
        {
            var hospitalAdminId = GetCurrentUserId();
            if (string.IsNullOrEmpty(hospitalAdminId))
            {
                return Unauthorized("Hospital ID could not be determined from token.");
            }

            try
            {
                var schedules = await _scheduleService.GetSchedulesForAdminAsync(hospitalAdminId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving schedules for hospital {HospitalId}.", hospitalAdminId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving schedules.");
            }
        }

 
        [HttpGet("{id}")]
        public async Task<ActionResult<ScheduleDto>> GetScheduleById(int id)
        {
            var hospitalAdminId = GetCurrentUserId();
            if (string.IsNullOrEmpty(hospitalAdminId))
            {
                return Unauthorized("Hospital ID could not be determined from token.");
            }

            try
            {
                var schedule = await _scheduleService.GetScheduleByIdForAdminAsync(id, hospitalAdminId);
                if (schedule == null)
                {
                    return NotFound($"Schedule with ID {id} not found or not accessible by your hospital.");
                }
                return Ok(schedule);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Admin {HospitalId} attempted to access schedule {ScheduleId} without permission.", hospitalAdminId, id);
                return Forbid("You do not have permission to view this schedule.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving schedule {ScheduleId} for hospital {HospitalId}.", id, hospitalAdminId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the schedule.");
            }
        }

        [HttpPut("ScheduleUpdate/{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("ID mismatch between route and payload.");
            }
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Validation errors during schedule update for ID {ScheduleId}: {Errors}", id, ModelState);
                return BadRequest(ModelState);
            }

            var hospitalAdminId = GetCurrentUserId();
            if (string.IsNullOrEmpty(hospitalAdminId))
            {
                _logger.LogWarning("Unauthorized attempt to update schedule: Hospital ID could not be determined from token.");
                return Unauthorized("Hospital ID could not be determined from token.");
            }

            try
            {
                var updated = await _scheduleService.UpdateScheduleByAdminAsync(updateDto, hospitalAdminId);
                if (!updated)
                {
                    return NotFound("Schedule not found or you do not have permission to update it.");
                }
                _logger.LogInformation("Admin {HospitalId} successfully updated schedule {ScheduleId}.", hospitalAdminId, id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Bad request during schedule update for ID {ScheduleId} by admin {HospitalId}: {Message}", id, hospitalAdminId, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Admin {HospitalId} attempted to update schedule {ScheduleId} without permission.", hospitalAdminId, id);
                return Forbid("You do not have permission to update this schedule.");
            }
            catch (InvalidOperationException ex) // Catch specific operation validation errors from service
            {
                _logger.LogWarning(ex, "Invalid operation during schedule update for ID {ScheduleId}: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during schedule update for ID {ScheduleId} by admin {HospitalId}.", id, hospitalAdminId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while updating the schedule.");
            }
        }

        [HttpDelete("ScheduleDelete/{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var hospitalAdminId = GetCurrentUserId();
            if (string.IsNullOrEmpty(hospitalAdminId))
            {
                _logger.LogWarning("Unauthorized attempt to delete schedule: Hospital ID could not be determined from token.");
                return Unauthorized("Hospital ID could not be determined from token.");
            }

            try
            {
                var deleted = await _scheduleService.DeleteScheduleByAdminAsync(id, hospitalAdminId);
                if (!deleted)
                {
                    return NotFound("Schedule not found or you do not have permission to delete it.");
                }
                _logger.LogInformation("Admin {HospitalId} successfully soft-deleted schedule {ScheduleId}.", hospitalAdminId, id);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Admin {HospitalId} attempted to delete schedule {ScheduleId} without permission.", hospitalAdminId, id);
                return Forbid("You do not have permission to delete this schedule.");
            }
            catch (InvalidOperationException ex) // Catch specific business rule violations from service
            {
                _logger.LogWarning(ex, "Invalid operation during schedule deletion for ID {ScheduleId}: {Message}", id, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during schedule deletion for ID {ScheduleId} by admin {HospitalId}.", id, hospitalAdminId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while deleting the schedule.");
            }
        }
    }
}