using Elagy.BL.Services;
using Elagy.Core.DTOs.Driver;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.User;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Authorization; // For [Authorize] attribute
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Protect this controller, allowing only ServiceProviders that are CarRental type
    [Authorize(Roles = "CarRentalServiceProvider")] // Assuming a generic ServiceProvider role for all providers
    public class CarRentalProviderController : BaseApiController
    {
        private readonly ICarRentalProviderService _carRentalProviderService;
        private readonly ICarDriverAssignmentService _carDriverAssignmentService;

        public CarRentalProviderController(ICarDriverAssignmentService carDriverAssignmentService,ICarRentalProviderService carRentalProviderService)
        {
            _carRentalProviderService = carRentalProviderService;
            _carDriverAssignmentService = carDriverAssignmentService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<CarRentalProviderProfileDto>> GetProfile()
        {
            var userId = GetCurrentUserId(); // Get current user's ID from token
            if (userId == null) return Unauthorized();

            var profile = await _carRentalProviderService.GetCarRentalProviderProfileAsync(userId);
            if (profile == null)
            {
                return NotFound("Car Rental provider profile not found or user is not a Car Rental provider.");
            }
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<CarRentalProviderProfileDto>> UpdateProfile([FromBody] CarRentalProviderProfileUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var updatedProfile = await _carRentalProviderService.UpdateCarRentalProviderProfileAsync(userId, model);
            if (updatedProfile == null)
            {
                return BadRequest("Failed to update car rental provider profile. Check if profile exists or data is valid.");
            }
            return Ok(updatedProfile);
        }


        [HttpPost]
        public async Task<IActionResult> AssignDriverToCar([FromBody] CarDriverCreateDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                var result = await _carDriverAssignmentService.AssignDriverToCarAsync(createDto, carRentalAssetId);
                return CreatedAtAction(nameof(GetAssignmentById), new { carDriverId = result.Id }, result);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred during assignment.");
            }
        }


        [HttpPut("release/{carId}/{driverId}")]
        public async Task<IActionResult> ReleaseDriverFromCar(int carId, string driverId)
        {

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                var result = await _carDriverAssignmentService.ReleaseDriverFromCarAsync(carId, driverId, carRentalAssetId);
                if (result == null) return NotFound($"Active assignment for Car {carId} and Driver {driverId} not found.");
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred during release.");
            }
        }



        [HttpGet("current-assignment-for-driver/{driverId}")]
        public async Task<IActionResult> GetCurrentAssignmentForDriver(string driverId)
        {
            if (string.IsNullOrWhiteSpace(driverId)) return BadRequest("Driver ID cannot be empty.");

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                var result = await _carDriverAssignmentService.GetCurrentAssignmentForDriverAsync(driverId, carRentalAssetId);
                if (result == null) return NotFound($"No active assignment found for Driver {driverId}.");
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }



        [HttpGet("assignments")]
        public async Task<IActionResult> GetAssignmentsForCarRentalAsset(
            [FromQuery] int PageNumber = 1,
            [FromQuery] int PageSize = 10, 
            [FromQuery] bool? IsAssignedCurrent = null)
        {
            if (PageNumber < 1 || PageSize < 1) return BadRequest("PageNumber and PageSize must be greater than 0.");

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    FilterIsActive = IsAssignedCurrent
                };

                var result = await _carDriverAssignmentService.GetAssignmentsForCarRentalAssetAsync(carRentalAssetId, paginationParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving assignments.");
            }
        }

        [HttpGet("assignment/{carDriverId}")]
        public async Task<IActionResult> GetAssignmentById(int carDriverId)
        {
            if (carDriverId <= 0) return BadRequest("Car Driver Assignment ID must be positive.");
            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                var result = await 
                    _carDriverAssignmentService.GetAssignmentsForCarRentalAssetAsync(
                    carRentalAssetId, new PaginationParameters { PageNumber = 1, PageSize = 100 });
                var specificAssignment = result.Items?.FirstOrDefault(a => a.Id == carDriverId);

                if (specificAssignment == null) return NotFound($"Assignment with ID {carDriverId} not found or not affiliated with your asset.");
                return Ok(specificAssignment);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }


     [HttpGet("history/driver/{driverId}")]
     public async Task<IActionResult> GetAssignmentHistoryForDriver(
     string driverId,
     [FromQuery] int PageNumber = 1,
     [FromQuery] int PageSize = 10,
     [FromQuery] int? CarId = null,
     [FromQuery] bool? IsAssignedCurrent = null)
        {
            if (string.IsNullOrWhiteSpace(driverId)) return BadRequest("Driver ID cannot be empty.");
            if (PageNumber < 1 || PageSize < 1) return BadRequest("PageNumber and PageSize must be greater than 0.");

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                };
                var result = await _carDriverAssignmentService.GetAssignmentHistoryForDriverAsync(driverId, carRentalAssetId, paginationParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving history.");
            }
        }



    }
}