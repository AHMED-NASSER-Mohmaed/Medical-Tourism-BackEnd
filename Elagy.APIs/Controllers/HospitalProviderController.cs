using Elagy.BL.Services;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Schedule;
using Elagy.Core.DTOs.User;
using Elagy.Core.IServices;
using Elagy.Core.IServices.ISpecialtyService;
using Microsoft.AspNetCore.Authorization;  
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]

    [Authorize(Roles = "HospitalServiceProvider")]
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

        [HttpGet("my-hospital")]
        public async Task<IActionResult> GetMyHospitalSchedules(
            [FromQuery] int PageNumber = 1,
            [FromQuery] int PageSize = 10,
            [FromQuery] string? SearchTerm = null,
            [FromQuery] int? FilterDayOfWeekId = null,
            [FromQuery] DateTime? FilterStartDate = null,
            [FromQuery] DateTime? FilterEndDate = null,
            [FromQuery] bool? FilterIsRecurring = null,
            [FromQuery] bool? FilterIsActive = null) 
        {
            var hospitalId = GetCurrentUserId();
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from your token.");

            if (PageNumber < 1 || PageSize < 1) return BadRequest("PageNumber and PageSize must be greater than 0.");

            try
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm,
                    FilterDayOfWeekId = FilterDayOfWeekId,
                    FilterStartDate = FilterStartDate,
                    FilterEndDate = FilterEndDate,
                    FilterIsRecurring = FilterIsRecurring,
                    FilterIsActive = FilterIsActive // Pass to service
                };

                _logger.LogInformation($"Received request for schedules for Hospital ID: {hospitalId} (Admin Dashboard).");
                var result = await _scheduleService.GetAllSchedulesForHospitalAdminAsync(hospitalId, paginationParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting schedules for Hospital ID: {hospitalId} (Admin Dashboard).");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving schedules.");
            }
        }

        [HttpPost("AddSchedule")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleSlotDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var hospitalId = GetCurrentUserId();
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to create schedule for Doctor {createDto.DoctorId} at HS {createDto.HospitalSpecialtyId}.");
                var result = await _scheduleService.CreateScheduleAsync(createDto, hospitalId);
                return CreatedAtAction(nameof(GetScheduleById), new { scheduleId = result.Id }, result);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex) 
            {
                return Forbid(ex.Message); 
            }
            catch (InvalidOperationException ex) 
            {
                return Conflict(ex.Message); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating schedule slot.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while creating the schedule.");
            }
        }

 
        [HttpPut("{scheduleId}")]
        [Authorize(Roles = "HospitalServiceProvider")]
        public async Task<IActionResult> UpdateSchedule(int scheduleId, [FromBody] UpdateScheduleDto updateDto)
        {
            if (scheduleId <= 0) return BadRequest("Schedule ID must be a positive integer.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var hospitalId = GetCurrentUserId();
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to update schedule ID: {scheduleId}.");
                var result = await _scheduleService.UpdateScheduleAsync(scheduleId, updateDto, hospitalId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex) 
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex) 
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating schedule ID: {scheduleId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while updating the schedule.");
            }
        }

     
        [HttpPut("changestatus/{scheduleId}")]
        public async Task<IActionResult> ChangeScheduleStatus(int scheduleId, [FromBody] bool newIsActiveStatus)
        {
            if (scheduleId <= 0) return BadRequest("Schedule ID must be a positive integer.");

            var hospitalId = GetCurrentUserId();
            if (hospitalId == null) return Unauthorized("Hospital ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to change status for schedule ID: {scheduleId} to {newIsActiveStatus}.");
                var result = await _scheduleService.ChangeScheduleStatusAsync(scheduleId, newIsActiveStatus, hospitalId);
                if (result == null) 
                {
                    return NotFound($"Schedule with ID {scheduleId} not found.");
                }
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex) 
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing status for schedule ID: {scheduleId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while changing schedule status.");
            }
        }


       
        [HttpGet("available-slots")]
        [AllowAnonymous] 
        public async Task<IActionResult> GetAvailablePatientSlots(
            [FromQuery] int PageNumber = 1,
            [FromQuery] int PageSize = 10,
            [FromQuery] string? SearchTerm = null,
            [FromQuery] int? SpecialtyId = null,
            [FromQuery] int? FilterDayOfWeekId = null,
            [FromQuery] DateTime? FilterStartDate = null,
            [FromQuery] DateTime? FilterEndDate = null,
            [FromQuery] string? FilterDoctorId = null)
        {
            if (PageNumber < 1 || PageSize < 1) return BadRequest("PageNumber and PageSize must be greater than 0.");

            try
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm,
                    SpecialtyId = SpecialtyId,
                    FilterDayOfWeekId = FilterDayOfWeekId,
                    FilterStartDate = FilterStartDate,
                    FilterEndDate = FilterEndDate,
                    FilterDoctorId = FilterDoctorId
                };

                _logger.LogInformation($"Received request for available patient slots. Page: {PageNumber}, Size: {PageSize}.");
                var result = await _scheduleService.GetAvailablePatientSlotsAsync(paginationParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available patient slots.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving available slots.");
            }
        }


        [HttpGet("{scheduleId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetScheduleById(int scheduleId)
        {
            if (scheduleId <= 0) return BadRequest("Schedule ID must be a positive integer.");
            try
            {
                var result = await _scheduleService.GetScheduleByIdAsync(scheduleId);
                if (result == null) return NotFound($"Schedule with ID {scheduleId} not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting schedule by ID: {scheduleId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving the schedule.");
            }
        }

    }
}
