using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Room;
using Elagy.Core.DTOs.User;
using Elagy.Core.Enums;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Authorization; // For [Authorize] attribute
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    // Protect this controller, allowing only ServiceProviders that are Hotel type
    [Authorize(Roles = "HotelServiceProvider")] // Assuming a generic ServiceProvider role for all providers
    public class HotelProviderController : ProfileImageBaseController
    {
        private readonly IHotelProviderService _hotelProviderService;
        private readonly IRoomService _roomService;

        public HotelProviderController(IRoomService roomService,IHotelProviderService hotelProviderService,IImageProfile _imageProfile,ILogger<HotelProviderController>logger)
        :base(_imageProfile, logger) 
        {
            _hotelProviderService = hotelProviderService;
            _roomService=roomService;
        }

        [HttpGet("profile")]
        public async Task<ActionResult<HotelProviderProfileDto>> GetProfile()
        {
            var userId = GetCurrentUserId(); // Get current user's ID from token
            if (userId == null) return Unauthorized();

            var profile = await _hotelProviderService.GetHotelProviderProfileAsync(userId);
            if (profile == null)
            {
                // Note: If profile is null, it could mean not found, or not the correct asset type for this controller.
                // You might want more specific error handling.
                return NotFound("Hotel provider profile not found or user is not a Hotel provider.");
            }
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<HotelProviderProfileDto>> UpdateProfile([FromBody] HotelProviderProfileUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var updatedProfile = await _hotelProviderService.UpdateHotelProviderProfileAsync(userId, model);
            if (updatedProfile == null)
            {
                return BadRequest("Failed to update hotel provider profile. Check if profile exists or data is valid.");
            }
            return Ok(updatedProfile);
        }

        // Add other Hotel Provider specific actions here (e.g., managing rooms, bookings, availability)

        [HttpPost("upload-hotel-images")]
        public async Task<ActionResult<List<AssetImageResponseDto>>> UploadHotelImages(
        [FromForm] List<IFormFile> hotelImages)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var hotelId = GetCurrentUserId();

            try
            {
                var result = await _hotelProviderService.UploadHotelAssetImages(hotelId, hotelImages);
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

        [HttpDelete("delete-selected-images")]
        public async Task<ActionResult<List<AssetImageResponseDto>>> DeletedHotelImages([FromBody] List<string> imageIds)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var hoteId = GetCurrentUserId();

            try
            {

                var result = await _hotelProviderService.DeleteHotelAssetImagesByIds(hoteId, imageIds);
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

        [HttpGet("Rooms")]

        public async Task<IActionResult> GetMyHotelRooms(
            [FromQuery] int PageNumber = 1,
            [FromQuery] int PageSize = 10,
            [FromQuery] string? SearchTerm = null,
            [FromQuery] RoomCategory? RoomType = null,
            [FromQuery] RoomStatus? RoomStatus = null,
            [FromQuery] bool? FilterIsAvailable = null,
            [FromQuery] decimal? MinPrice = null,
            [FromQuery] decimal? MaxPrice = null,
            [FromQuery] int? MinOccupancy = null,
            [FromQuery] int? MaxOccupancy = null,
            [FromQuery] int? FilterGovernorateId = null)
        {
            var hotelAssetId = GetCurrentUserId();
            if (hotelAssetId == null) return Unauthorized("Hotel ID could not be determined from your token.");

            if (PageNumber < 1 || PageSize < 1) return BadRequest("PageNumber and PageSize must be greater than 0.");

            try
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm,
                    RoomType = RoomType,
                    RoomStatus = RoomStatus,
                    FilterIsActive = FilterIsAvailable,
                    MinPrice = MinPrice,
                    MaxPrice = MaxPrice,
                    MinOccupancy = MinOccupancy,
                    MaxOccupancy = MaxOccupancy,
                    FilterGovernorateId = FilterGovernorateId
                };

                _logger.LogInformation($"Received request for rooms for Hotel ID: {hotelAssetId} (Admin Dashboard).");
                var result = await _roomService.GetAllRoomsForHotelAdminAsync(hotelAssetId, paginationParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting rooms for Hotel ID: {hotelAssetId} (Admin Dashboard).");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving rooms.");
            }
        }

      

        [HttpPost("Room")]
        public async Task<IActionResult> CreateRoom([FromForm] RoomCreateDto createDto, [FromForm] IFormFile[]? imageFiles)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var hotelAssetId = GetCurrentUserId();
            if (hotelAssetId == null) return Unauthorized("Hotel ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to create room '{createDto.RoomNumber}' for Hotel {hotelAssetId}.");
                var filesList = imageFiles?.ToList();
                var result = await _roomService.CreateRoomAsync(createDto, hotelAssetId, filesList);
                return CreatedAtAction(nameof(GetRoomById), new { roomId = result.Id }, result);
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
                _logger.LogError(ex, $"Error creating room '{createDto.RoomNumber}'.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while creating the room.");
            }
        }

        [HttpPut("Room/{roomId}")]
        public async Task<IActionResult> UpdateRoom(int roomId, [FromForm] RoomUpdateDto updateDto, [FromForm] IFormFile[]? newImageFiles)
        {
            if (roomId <= 0) return BadRequest("Room ID must be a positive integer.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var hotelAssetId = GetCurrentUserId();
            if (hotelAssetId == null) return Unauthorized("Hotel ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to update room ID: {roomId} for Hotel {hotelAssetId}.");
                var filesList = newImageFiles?.ToList();
                var result = await _roomService.UpdateRoomAsync(roomId, updateDto, hotelAssetId, filesList);
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
                _logger.LogError(ex, $"Error updating room ID: {roomId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while updating the room.");
            }
        }
        [HttpGet("Room/{roomId}")]
        public async Task<IActionResult> GetRoomById(int roomId)
        {
            if (roomId <= 0) return BadRequest("Room ID must be a positive integer.");
            try
            {
                var result = await _roomService.GetRoomByIdAsync(roomId);
                if (result == null) return NotFound($"Room with ID {roomId} not found.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting room by ID: {roomId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving the room.");
            }
        }

        [HttpPut("Room/Instatus/{roomId}")]
        public async Task<IActionResult> ChangeRoomInternalStatus(int roomId, [FromBody] RoomStatus newInternalStatus)
        {
            if (roomId <= 0) return BadRequest("Room ID must be a positive integer.");

            var hotelAssetId = GetCurrentUserId();
            if (hotelAssetId == null) return Unauthorized("Hotel ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to change internal status for room ID: {roomId} to {newInternalStatus}.");
                var result = await _roomService.ChangeRoomInternalStatusAsync(roomId, newInternalStatus, hotelAssetId);
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
                _logger.LogError(ex, $"Error changing internal status for room ID: {roomId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while changing room internal status.");
            }
        }

        [HttpPut("Room/availability/{roomId}")]
        public async Task<IActionResult> ChangeRoomAvailabilityStatus(int roomId, [FromBody] bool newIsAvailableStatus)
        {
            if (roomId <= 0) return BadRequest("Room ID must be a positive integer.");

            var hotelAssetId = GetCurrentUserId();
            if (hotelAssetId == null) return Unauthorized("Hotel ID could not be determined from your token.");

            try
            {
                _logger.LogInformation($"Received request to change availability for room ID: {roomId} to {newIsAvailableStatus}.");
                var result = await _roomService.ChangeRoomAvailabilityStatusAsync(roomId, newIsAvailableStatus, hotelAssetId);
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
                _logger.LogError(ex, $"Error changing availability for room ID: {roomId}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while changing room availability.");
            }
        }
    }
}