using Elagy.Core.DTOs.Doctor;
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
    public class WebsiteController : BaseApiController
    {
        private readonly IHospitalProviderService _hospitalProviderService;
        private readonly ICarRentalProviderService _carRentalProviderService;
        private readonly IHotelProviderService _hotelProviderService;
        private readonly IRoomService _roomService;
        private readonly ISpecialtyService _specialtyService;
        private readonly IDoctorService _doctorService;
        private readonly ISpecialtyScheduleService _scheduleService;
        private readonly ISuperAdminService _superAdminService;
        private readonly ICarService _carservice;


        public WebsiteController(
            IHospitalProviderService hospitalProviderService,
            ICarRentalProviderService carRentalProviderService,
            IHotelProviderService hotelProviderService,
            IRoomService roomService,
            ISpecialtyService specialtyService,
            IDoctorService doctorService,
            ISpecialtyScheduleService scheduleService,
            ISuperAdminService superAdminService,
            ICarService carservice)
        {
            _hospitalProviderService = hospitalProviderService;
            _carRentalProviderService = carRentalProviderService;
            _hotelProviderService = hotelProviderService;
            _roomService = roomService;
            _specialtyService = specialtyService;
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _superAdminService = superAdminService;
            _carservice=carservice;
        }


        // still needd to maintain
        [HttpGet("CarRentals")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<HotelProviderProfileDto>>> GetCarRentalProviders(
          [FromQuery] int ?governertaeId=null,
        [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10,
        [FromQuery] string SearchTerm = null, [FromQuery] Status? UserStatus = null)
        {
            var Filter = new PaginationParameters();
            Filter.PageNumber = PageNumber;
            Filter.PageSize = PageSize;
            Filter.SearchTerm = SearchTerm;
            Filter.UserStatus = UserStatus;
            Filter.FilterGovernorateId = governertaeId;

            var providers = await _superAdminService.GetCarRentalProvidersForAdminDashboardAsync(Filter);
            return Ok(providers);
        }
        [HttpGet("CarAvailable/{carRentalId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableCarsForWebsite(string carRentalId,
              [FromQuery] int PageNumber = 1,
              [FromQuery] int PageSize = 10,
              [FromQuery] string? SearchTerm = null,
              [FromQuery] CarType? CarType = null,
              [FromQuery] decimal? MinPrice = null,
              [FromQuery] decimal? MaxPrice = null)
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
                    MaxPrice = MaxPrice
                };

                var result = await _carservice.GetAvailableCarsForWebsiteAsync(paginationParams, carRentalId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving available cars.");
            }
        }

        [HttpGet("hotels")]
        [AllowAnonymous]

        public async Task<ActionResult<IEnumerable<HotelProviderProfileDto>>> GetHotelProviders(
         [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10,
         [FromQuery] string SearchTerm = null
        , [FromQuery] int? GovernerateId = null)
        {
            var Filter = new PaginationParameters();
            Filter.PageNumber = PageNumber;
            Filter.PageSize = PageSize;
            Filter.SearchTerm = SearchTerm;
            Filter.FilterGovernorateId = GovernerateId;

            var providers = await _superAdminService.GetHotelProvidersForAdminDashboardAsync(Filter);
            return Ok(providers);
        }

        [HttpGet("Rooms-Website/{hotellId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableRoomsForWebsite(string hotellId,
           [FromQuery] int PageNumber = 1,
           [FromQuery] int PageSize = 10,
           [FromQuery] string? SearchTerm = null,
           [FromQuery] RoomCategory? RoomType = null,
           [FromQuery] decimal? MinPrice = null,
           [FromQuery] decimal? MaxPrice = null,
           [FromQuery] int? MinOccupancy = null,
           [FromQuery] int? MaxOccupancy = null,
           [FromQuery] int? FilterGovernorateId = null
          )
        {
            if (PageNumber < 1 || PageSize < 1) return BadRequest("PageNumber and PageSize must be greater than 0.");

            try
            {
                var paginationParams = new PaginationParameters
                {
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    SearchTerm = SearchTerm,
                    RoomType = RoomType,
                    MinPrice = MinPrice,
                    MaxPrice = MaxPrice,
                    MinOccupancy = MinOccupancy,
                    MaxOccupancy = MaxOccupancy,
                    FilterGovernorateId = FilterGovernorateId
                };

                var result = await _roomService.GetAvailableRoomsForWebsiteAsync(paginationParams, hotellId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving available rooms.");
            }
        }


        [HttpGet("Room/{roomId}")]
        [AllowAnonymous]

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
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving the room.");
            }
        }

      

       


        [HttpGet("Hospitals")]
        [AllowAnonymous]

        public async Task<ActionResult<IEnumerable<HotelProviderProfileDto>>> GetHospitalProviders(
      [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10,
      [FromQuery] string SearchTerm = null, [FromQuery] Status? UserStatus = Status.Active,
      [FromQuery] int? specialtyId = null,
      [FromQuery] int? GovernerateId = null)
        {
            var Filter = new PaginationParameters();
            Filter.PageNumber = PageNumber;
            Filter.PageSize = PageSize;
            Filter.SearchTerm = SearchTerm;
            Filter.UserStatus = UserStatus;
            Filter.SpecialtyId = specialtyId;
            Filter.FilterGovernorateId = GovernerateId;


            var providers = await _superAdminService.GetHospitalProvidersForAdminDashboardAsync(Filter);
            return Ok(providers);
        }

        [HttpGet("Specilties")]
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


    [HttpGet("Specilties-in-Hospital/{hospitalId}")]
    public async Task<IActionResult> GetSpecialtiesByHospitalId(
    string hospitalId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? searchTerm = null,
    [FromQuery] bool? isActive = null,
    [FromQuery] int? specialtyId = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("PageNumber and PageSize must be greater than 0.");
            }

            try
            {
                var paginationParameters = new PaginationParameters
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    FilterIsActive = isActive,
                    SpecialtyId = specialtyId
                };

                var result = await _specialtyService.GetAllSpecialtiesForHospital(hospitalId, paginationParameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }


      

        [HttpGet("Doctors-in-Specialty/{specialtyId}/{hospitalId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorsByHospitalAndSpecialty(
                    string hospitalId,
                    int specialtyId,
                   [FromQuery] int PageNumber = 1,
                   [FromQuery] int PageSize = 10,
                   [FromQuery] string? SearchTerm = null)
        {
            if (string.IsNullOrWhiteSpace(hospitalId) || specialtyId <= 0 || PageNumber < 1 || PageSize < 1)
            {
                return BadRequest("Invalid parameters. Hospital ID, Specialty ID, PageNumber, and PageSize are required.");
            }
            try
            {
                var paginationParams = new PaginationParameters { PageNumber = PageNumber, PageSize = PageSize, SearchTerm = SearchTerm };
                var result = await _doctorService.GetAllDoctorsPerHospitalSpecialty(hospitalId, specialtyId, paginationParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving doctors.");
            }
        }





        //[HttpGet("schedule/available-slots")]
        //   [AllowAnonymous] 
        //   public async Task<IActionResult> GetAvailablePatientSlots(
        //       [FromQuery] int PageNumber = 1,
        //       [FromQuery] int PageSize = 10,
        //       [FromQuery] string? SearchTerm = null,
        //       [FromQuery] int? SpecialtyId = null,
        //       [FromQuery] int? FilterDayOfWeekId = null,
        //       [FromQuery] string? FilterDoctorId = null)
        //   {
        //       if (PageNumber < 1 || PageSize < 1) return BadRequest("PageNumber and PageSize must be greater than 0.");

        //       try
        //       {
        //           var paginationParams = new PaginationParameters
        //           {
        //               PageNumber = PageNumber,
        //               PageSize = PageSize,
        //               SearchTerm = SearchTerm,
        //               SpecialtyId = SpecialtyId,
        //               FilterDayOfWeekId = FilterDayOfWeekId,
        //               FilterDoctorId = FilterDoctorId
        //           };

        //           var result = await _scheduleService.GetAvailablePatientSlotsAsync(paginationParams);
        //           return Ok(result);
        //       }
        //       catch (Exception ex)
        //       {
        //           return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred while retrieving available slots.");
        //       }
        //   }


        [HttpGet("doctors/available-schedules/{doctorId}")]
    public async Task<IActionResult> GetAvailableSchedulesByDoctor(
    string doctorId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? searchTerm = null,
    [FromQuery] int? filterDayOfWeekId = null
)
        {
            var paginationParameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                FilterDayOfWeekId = filterDayOfWeekId,
            };

            var result = await _scheduleService.GetAvailablePatientSlotsAsync(doctorId, paginationParameters);
            return Ok(result);
        }
    }
    }
