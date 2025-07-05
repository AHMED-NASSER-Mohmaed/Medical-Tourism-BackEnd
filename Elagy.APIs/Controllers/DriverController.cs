using Elagy.BL.Services;
using Elagy.Core.DTOs.Driver;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Enums;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : BaseApiController
    {

        private readonly IDriverService _driverService;
        private readonly ILogger<DriverController> _logger;

        public DriverController(IDriverService driverService, IImageProfile _imageProfile, ILogger<DriverController> logger)

        {
            _driverService = driverService;
            _logger = logger;
        }

        [HttpGet("my-driver-car-rental")]
        [Authorize(Roles = "CarRentalServiceProvider")]

        public async Task<IActionResult> GetMyCarRentalDrivers(
            [FromQuery] int PageNumber = 1,
            [FromQuery] int PageSize = 10,
            [FromQuery] string? SearchTerm = null,
            [FromQuery] DriverStatus? DriverStatus = null)
        {
            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            if (PageNumber < 1 || PageSize < 1) return BadRequest("PageNumber and PageSize must be greater than 0.");

            try
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm,
                    DriverStatus = DriverStatus,

                };

                _logger.LogInformation($"Received request for drivers for Car Rental Asset ID: {carRentalAssetId} (Admin Dashboard).");
                var result = await _driverService.GetAllDriversForCarRentalAdminAsync(carRentalAssetId, paginationParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting drivers for Car Rental Asset ID: {carRentalAssetId} (Admin Dashboard).");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving drivers.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "CarRentalServiceProvider")]
        public async Task<IActionResult> CreateDriver(
            [FromForm] DriverCreateDto createDto,
             IFormFile? licenseDocument,
             IFormFile? profileImage)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to create driver for Car Rental Asset {createDto.CarRentalAssetId}. Email: {createDto.Email}");

                var result = await _driverService.CreateDriverAsync(createDto, carRentalAssetId, licenseDocument, profileImage);

                return CreatedAtAction(nameof(GetDriverById), new { driverId = result.Id }, result);
            }
  
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating driver for Car Rental Asset {createDto.CarRentalAssetId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while creating the driver.");
            }
        }


        [HttpPut("{driverId}")]
        [Authorize(Roles = "CarRentalServiceProvider")]
        public async Task<IActionResult> UpdateDriver(
            string driverId,
            [FromForm] DriverUpdateDto updateDto,
             IFormFile? newLicenseDocument,
             IFormFile? newProfileImage)
        {
            if (string.IsNullOrWhiteSpace(driverId)) return BadRequest("Driver ID cannot be empty.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to update driver ID: {driverId} for Car Rental Asset {carRentalAssetId}.");
                var result = await _driverService.UpdateDriverAsync(driverId, updateDto, carRentalAssetId, newLicenseDocument, newProfileImage);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating driver ID: {driverId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while updating the driver.");
            }
        }

        [HttpDelete("change-driver-activation/{driverId}")]
        [Authorize(Roles = "CarRentalServiceProvider")]
        public async Task<IActionResult> DeleteDriver(string driverId)
        {
            if (string.IsNullOrWhiteSpace(driverId)) return BadRequest("Driver ID cannot be empty.");

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                var result = await _driverService.DeleteDriverAsync(driverId, carRentalAssetId);
                if (result == null) return NotFound($"Driver with ID {driverId} not found.");
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting driver ID: {driverId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while deleting the driver.");
            }
        }
        [HttpPut("status/{DriverId}")]
        [Authorize(Roles = "CarRentalServiceProvider")]
        public async Task<IActionResult> ChangeDriverInternalStatus(string DriverId, [FromBody] DriverStatus newDriverStatus)
        {
            if (string.IsNullOrWhiteSpace(DriverId)) return BadRequest("Driver ID cannot be empty.");

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                var result = await _driverService.ChangeDriverInternalStatusAsync(DriverId, newDriverStatus, carRentalAssetId);
                if (result == null) return NotFound($"Driver with ID {DriverId} not found.");
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing internal status for driver ID: {DriverId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while changing driver internal status.");
            }
        }

        [HttpGet("{driverId}")]
        [Authorize(Roles = "CarRentalServiceProvider")]
        public async Task<IActionResult> GetDriverById(string driverId)
        {
            if (string.IsNullOrWhiteSpace(driverId)) return BadRequest("Driver ID cannot be empty.");
            try
            {
                var result = await _driverService.GetDriverByIdAsync(driverId);
                if (result == null) return NotFound($"Driver with ID {driverId} not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting driver by ID: {driverId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving the driver.");
            }
        }
    }
}
