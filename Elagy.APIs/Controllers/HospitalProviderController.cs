using Elagy.BL.Services;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Schedule;
using Elagy.Core.DTOs.User;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportProHtmlToPdf.IServices;
using ReportProHtmlToPdf.Model;
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]

    [Authorize(Roles = "HospitalServiceProvider")]
    public class HospitalProviderController : ProfileImageBaseController
    {
        private readonly IHospitalProviderService _hospitalProviderService;
        private readonly ISpecialtyScheduleService _scheduleService;
        private readonly IReportProHtmlToPdf _reportProHtmlToPdf;

        public HospitalProviderController(
            IReportProHtmlToPdf reportProHtmlToPdf,
            IImageProfile profileImageService,
             ISpecialtyScheduleService scheduleService,
            IHospitalProviderService hospitalProviderService,
            ILogger<HospitalProviderController> logger)
            : base(profileImageService, logger)
        {
            _hospitalProviderService = hospitalProviderService;
            _scheduleService = scheduleService;
            _reportProHtmlToPdf = reportProHtmlToPdf;
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

        [HttpGet("Hospital-Schedules")]
        public async Task<IActionResult> GetMyHospitalSchedules(
            [FromQuery] int PageNumber = 1,
            [FromQuery] int PageSize = 10,
            [FromQuery] string? SearchTerm = null,
            [FromQuery] int? FilterDayOfWeekId = null,
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

        [HttpGet("hospital-appointments")]
        public async Task<IActionResult> GetHospitalAppointments([FromQuery] AppointmentStatus? appointmentStatus = null,
            [FromQuery] DateOnly? Date = null, [FromQuery] int? specialtyScheduleId=null)
        {
            var hospitalAssetId = GetCurrentUserId();
            PaginationParameters paginationParameters = new PaginationParameters();
            paginationParameters.FilterStartDate = Date;
            paginationParameters.AppointmentStatus= appointmentStatus;
             paginationParameters.specialtyScheduleId= specialtyScheduleId;


            var result = await _hospitalProviderService.GetHospitalAppointmentsAsync(hospitalAssetId, paginationParameters);

            if (!result.Items.Any())
                return NotFound("No appointments found for this hospital.");

            return Ok(result);
        }
        [HttpPost("upload-hospital-images")]
        public async Task<ActionResult<List<AssetImageResponseDto>>> UploadHospitalImages([FromForm] List<IFormFile> hospitalImages)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var hospitalId = GetCurrentUserId();

            try
            {
                var result = await _hospitalProviderService.UploadHospitalAssetImages(hospitalId, hospitalImages);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        [HttpDelete("delete-hospital-images")]
        public async Task<ActionResult<List<AssetImageResponseDto>>> DeleteSelectedHospitalImages( [FromBody] List<string> imageIds)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var hospitalId = GetCurrentUserId();

            try
            {
                var result = await _hospitalProviderService.DeleteHospitalAssetImagesByIds(hospitalId, imageIds);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        // POST: api/Schedules
        [HttpPost("schedule")]
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

        // PUT: api/Schedules/{scheduleId}
        [HttpPut("schedule/{scheduleId}")]
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

        // PUT: api/Schedules/{scheduleId}/status
        [HttpPut("schedule/status/{scheduleId}")]
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


        [HttpGet("schedule/available-slots")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailablePatientSlots(
            [FromQuery] string FilterDoctorId)
        {
            try
            {
                var result = await _scheduleService.GetAvailablePatientSlotsAsync(FilterDoctorId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available patient slots.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving available slots.");
            }
        }


        // GET: api/Schedules/{scheduleId}
        [HttpGet("schedule/{scheduleId}")]
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
        [HttpGet("disbursement")]
        [Authorize(Roles = "HospitalServiceProvider")]
        public async Task<IActionResult> GetAllDisbursement([FromHeader]PaginationParameters pagination) 
        {
            Console.WriteLine("inside Controller=======================================================================================");
            var userId = GetCurrentUserId(); // Get current user's ID from token
            Console.WriteLine("After get user");
            Console.WriteLine("userId"+userId);
            if (userId == null) return Unauthorized();
            try
            {
                // Assuming you have a method to get disbursement details
                var disbursementDetails = await _hospitalProviderService.GetDisbursement(userId, pagination);
                if (disbursementDetails == null)
                {
                    return NotFound("Disbursement details not found for the user.");
                }
                return Ok(disbursementDetails);
            }
            catch (Exception ex) 
            {

                _logger.LogError(ex, $"Error getting disbursement");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving the schedule.");
            }

        }

        [HttpGet("disbursement/{id}")]
        public async Task<IActionResult> GetDisbursementByID(int id) 
        {
            try 
            {
                Console.WriteLine("in controller DisID"+ id);
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();
                var disbursement = await _hospitalProviderService.GetDisbursementWithDetails(id, userId);
                Console.WriteLine("before send Response =====================================================");
                //foreach (var item in disbursement.DisbursementItems)
                //{
                //    Console.WriteLine($"Item ID: {item.Id}, Amount: {item.Amount}");

                //    Console.WriteLine($"itemAppointement : {item.Appointment.Id } - {item.Appointment.MeetingUrl} - {item.Appointment.Date}");
                //    Console.WriteLine("Finish FirstItem");
                //}
                Console.WriteLine("send");
                return Ok(disbursement);

            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, $"Error getting disbursement");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving the schedule.");
            }
        }

        [HttpGet("disbursement/Report/{id}")]

        public async Task<IActionResult> PrintReport(int id) 
        {
            try 
            {
                var userId = GetCurrentUserId();
                if (userId == null) return Unauthorized();
                var disbursement = await _hospitalProviderService.GetDisbursementWithDetails( id, userId);

               
                var templatePath = "/Views/Reports/DisbursementHospitalReport.cshtml";  // keep this path correct
                var printPdf = await _reportProHtmlToPdf.RenderToPdfAsync(templatePath, disbursement);


                return File(printPdf.Pdf, printPdf.ContentType, printPdf.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting disbursement");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving the schedule.");
            }
        }
        

    }
}
