using Elagy.Core.DTOs.CarRentals;
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
    public class CarController : BaseApiController
    {
        private readonly ICarService _carService;
        private readonly ILogger<CarController> _logger;

        public CarController(ICarService carService, ILogger<CarController> logger)
        {
            _carService = carService;
            _logger = logger;
        }


        [HttpGet("my-car-rental")]
        [Authorize(Roles = "CarRentalServiceProvider")]
        public async Task<IActionResult> GetMyCarRentalCars(
           [FromQuery] int PageNumber = 1,
           [FromQuery] int PageSize = 10,
           [FromQuery] string? SearchTerm = null,
           [FromQuery] CarType? CarType = null,
           [FromQuery] CarStatus? CarStatus = null,
           [FromQuery] bool? FilterIsActive = null,
           [FromQuery] decimal? MinPrice = null,
           [FromQuery] decimal? MaxPrice = null,
           [FromQuery] int? MinOccupancy = null,
           [FromQuery] int? MaxOccupancy = null,
           [FromQuery] int? FilterGovernorateId = null)
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
                    CarType = CarType,
                    CarStatus = CarStatus,
                    FilterIsActive = FilterIsActive,
                    MinPrice = MinPrice,
                    MaxPrice = MaxPrice,
                    MinOccupancy = MinOccupancy,
                    MaxOccupancy = MaxOccupancy,
                    FilterGovernorateId = FilterGovernorateId
                };

                _logger.LogInformation($"Received request for cars for Car Rental Asset ID: {carRentalAssetId} (Admin Dashboard).");
                var result = await _carService.GetAllCarsForCarRentalAdminAsync(carRentalAssetId, paginationParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cars for Car Rental Asset ID: {carRentalAssetId} (Admin Dashboard).");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving cars.");
            }
        }
        [HttpGet("CarAvailable/{carRentalId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableCarsForWebsite(string carRentalId,
           [FromQuery] int PageNumber = 1,
           [FromQuery] int PageSize = 10,
           [FromQuery] string? SearchTerm = null,
           [FromQuery] CarType? CarType = null,
           [FromQuery] decimal? MinPrice = null,
           [FromQuery] decimal? MaxPrice = null,
           [FromQuery] int? MinOccupancy = null,
           [FromQuery] int? MaxOccupancy = null)
        {
            if (PageNumber < 1 || PageSize < 1) return BadRequest("PageNumber and PageSize must be greater than 0.");

            try
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm,
                    CarType = CarType,
                    MinPrice = MinPrice,
                    MaxPrice = MaxPrice,
                    MinOccupancy = MinOccupancy,
                    MaxOccupancy = MaxOccupancy,
                };

                _logger.LogInformation($"Received request for available cars for website. Page: {PageNumber}, Size: {PageSize}.");
                var result = await _carService.GetAvailableCarsForWebsiteAsync(paginationParams, carRentalId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available cars for website.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving available cars.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "CarRentalServiceProvider")]
        public async Task<IActionResult> CreateCar(
            [FromForm] CarCreateDto createDto,
            [FromForm] List<IFormFile>? imageFiles)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to create car '{createDto.FactoryMake} {createDto.ModelName}' for Hotel {carRentalAssetId}.");
                var result = await _carService.CreateCarAsync(createDto, carRentalAssetId, imageFiles);
                return CreatedAtAction(nameof(GetCarById), new { carId = result.Id }, result);
            }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating car '{createDto.FactoryMake} {createDto.ModelName}'.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while creating the car.");
            }
        }

        [HttpPut("{carId}")]
        [Authorize(Roles = "CarRentalServiceProvider")]
        public async Task<IActionResult> UpdateCar(
            int carId,
            [FromForm] CarUpdateDto updateDto,
            [FromForm] List<IFormFile>? newImageFiles)
        {
            if (carId <= 0) return BadRequest("Car ID must be a positive integer.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to update car ID: {carId} for Car Rental Asset {carRentalAssetId}.");
                var result = await _carService.UpdateCarAsync(carId, updateDto, carRentalAssetId, newImageFiles);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating car ID: {carId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while updating the car.");
            }
        }


        [HttpPut("Change-internal-status/{carId}")]
        [Authorize(Roles = "CarRentalServiceProvider")]

        public async Task<IActionResult> ChangeCarInternalStatus(int carId, [FromBody] CarStatus newInternalStatus)
        {
            if (carId <= 0) return BadRequest("Car ID must be a positive integer.");

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to change internal status for car ID: {carId} to {newInternalStatus}.");
                var result = await _carService.ChangeCarInternalStatusAsync(carId, newInternalStatus, carRentalAssetId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing internal status for car ID: {carId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while changing car internal status.");
            }
        }

        [HttpPut("availability/{carId}")]
        [Authorize(Roles = "CarRentalServiceProvider")]

        public async Task<IActionResult> ChangeCarAvailabilityStatus(int carId, [FromBody] bool newIsAvailableStatus)
        {
            if (carId <= 0) return BadRequest("Car ID must be a positive integer.");

            var carRentalAssetId = GetCurrentUserId();
            if (carRentalAssetId == null) return Unauthorized("Car Rental Admin ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to change availability for car ID: {carId} to {newIsAvailableStatus}.");
                var result = await _carService.ChangeCarAvailabilityStatusAsync(carId, newIsAvailableStatus, carRentalAssetId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
            catch (ArgumentException ex) { return BadRequest(ex.Message); }
            catch (InvalidOperationException ex) { return Conflict(ex.Message); }
            catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing availability for car ID: {carId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while changing car availability.");
            }
        }

        [HttpGet("{carId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCarById(int carId)
        {
            if (carId <= 0) return BadRequest("Car ID must be a positive integer.");
            try
            {
                var result = await _carService.GetCarByIdAsync(carId);
                if (result == null) return NotFound($"Car with ID {carId} not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting car by ID: {carId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving the car.");
            }
        }

    }
}
