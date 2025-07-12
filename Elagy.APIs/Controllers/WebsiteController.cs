using Elagy.BL.Services;
using Elagy.Core.DTOs.Doctor;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.TOP;
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
        private readonly ICarScheduleService _carScheduleService;


        private readonly ICarService _carservice;
        private readonly IRoomScheduleService _RoomscheduleService;
        private readonly IServiceProvidersWebsiteService _serviceproviderwebsite;
        public WebsiteController(
            IHospitalProviderService hospitalProviderService,
            ICarRentalProviderService carRentalProviderService,
            IHotelProviderService hotelProviderService,
            IRoomService roomService,
            ISpecialtyService specialtyService,
            IDoctorService doctorService,
            ISpecialtyScheduleService scheduleService,

            ICarService carservice,
            IRoomScheduleService roomscheduleService,
            IServiceProvidersWebsiteService serviceproviderwebsite,
            ICarScheduleService carScheduleService)
        {
            _hospitalProviderService = hospitalProviderService;
            _carRentalProviderService = carRentalProviderService;
            _hotelProviderService = hotelProviderService;
            _roomService = roomService;
            _specialtyService = specialtyService;
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _carservice = carservice;
            _RoomscheduleService = roomscheduleService;
            _serviceproviderwebsite = serviceproviderwebsite;
            _carScheduleService = carScheduleService;
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

            var providers = await _serviceproviderwebsite.GetCarRentalProvidersForAdminDashboardAsync(Filter);
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


        [HttpGet("Top-Cars")]
        public async Task<IActionResult> GetTopRentedCars()
        {
            try
            {
                var topCars = await _carservice.GetTopCarsByRentals();
                return Ok(topCars);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("unavailable-dates/{carId}")]
        public async Task<IActionResult> GetUnavailableDatesForCar(int carId)
        {
            try
            {
                var result = await _carScheduleService.GetAvailableCarsSchedules(carId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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

            var providers = await _serviceproviderwebsite.GetHotelProvidersForAdminDashboardAsync(Filter);
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



        [HttpGet("rooms-unavailable-dates/{roomId}")]
        public async Task<IActionResult> GetFutureUnavailableDates(int roomId)
        {
            try
            {
                var result = await _RoomscheduleService.GetAvailableRoomsSchedules(roomId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("Top-Hotels")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopBookedHotels()
        {
            try
            {
                var topHotels = await _roomService.GetTopHotelsByBookings();
                return Ok(topHotels);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
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


            var providers = await _serviceproviderwebsite.GetHospitalProvidersForAdminDashboardAsync(Filter);
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

        [HttpGet("Top-Doctors")]
        [ProducesResponseType(typeof(IEnumerable<DoctorBookingCountDto>), 200)]
        public async Task<IActionResult> GetTopBookedDoctors()
        {
            try
            {
                var topDoctors = await _doctorService.GetTop3DoctorsByBookings();
                return Ok(topDoctors);
            }
            catch (Exception ex)
            {
                // Log error
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpGet("specialties-top-booked")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopBookedSpecialties()
        {
            try
            {
                var topSpecialties = await _specialtyService.GetTopSpecialtiesByBookings();
                return Ok(topSpecialties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
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


        //    [HttpGet("doctors/available-schedules/{doctorId}")]
        //public async Task<IActionResult> GetAvailableSchedulesByDoctor(
        //string doctorId,
        //[FromQuery] int pageNumber = 1,
        //[FromQuery] int pageSize = 10,
        //[FromQuery] string? searchTerm = null,
        //[FromQuery] int? filterDayOfWeekId = null)
        //    {
        //        var paginationParameters = new PaginationParameters
        //        {
        //            PageNumber = pageNumber,
        //            PageSize = pageSize,
        //            SearchTerm = searchTerm,
        //            FilterDayOfWeekId = filterDayOfWeekId,
        //        };

        //        var result = await _scheduleService.GetAvailablePatientSlotsAsync(doctorId, paginationParameters);
        //        return Ok(result);
        //    }
    }
}
