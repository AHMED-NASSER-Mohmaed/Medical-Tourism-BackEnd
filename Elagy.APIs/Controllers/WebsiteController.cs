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

        public WebsiteController(
            IHospitalProviderService hospitalProviderService,
            ICarRentalProviderService carRentalProviderService,
            IHotelProviderService hotelProviderService,
            IRoomService roomService,
            ISpecialtyService specialtyService,
            IDoctorService doctorService,
            ISpecialtyScheduleService scheduleService,
            ISuperAdminService superAdminService)
        {
            _hospitalProviderService = hospitalProviderService;
            _carRentalProviderService = carRentalProviderService;
            _hotelProviderService = hotelProviderService;
            _roomService = roomService;
            _specialtyService = specialtyService;
            _doctorService = doctorService;
            _scheduleService = scheduleService;
            _superAdminService = superAdminService;
        }

     

        [HttpGet("hotels")]
        [AllowAnonymous]

        public async Task<ActionResult<IEnumerable<HotelProviderProfileDto>>> GetHotelProviders(
         [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10,
         [FromQuery] string SearchTerm = null, [FromQuery] Status? UserStatus = null
        , [FromQuery] int? GovernerateId = null)
        {
            var Filter = new PaginationParameters();
            Filter.PageNumber = PageNumber;
            Filter.PageSize = PageSize;
            Filter.SearchTerm = SearchTerm;
            Filter.UserStatus = UserStatus;
            Filter.FilterGovernorateId = GovernerateId;

            var providers = await _superAdminService.GetHotelProvidersForAdminDashboardAsync(Filter);
            return Ok(providers);
        }

        // still needd to maintain
        [HttpGet("CarRentals")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<HotelProviderProfileDto>>> GetCarRentalProviders(
        [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10,
        [FromQuery] string SearchTerm = null, [FromQuery] Status? UserStatus = null)
        {
            var Filter = new PaginationParameters();
            Filter.PageNumber = PageNumber;
            Filter.PageSize = PageSize;
            Filter.SearchTerm = SearchTerm;
            Filter.UserStatus = UserStatus;

            var providers = await _superAdminService.GetCarRentalProvidersForAdminDashboardAsync(Filter);
            return Ok(providers);
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


        [HttpGet("Doctors-in-Specialty/{hospitalSpecialtyId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDoctorsByHospitalSpecialty(
           int hospitalSpecialtyId,
           [FromQuery] int PageNumber = 1,
           [FromQuery] int PageSize = 10,
           [FromQuery] string? SearchTerm = null,
           [FromQuery] int? SpecialtyId = null)

        {
            if (hospitalSpecialtyId <= 0 || PageNumber < 1 || PageSize < 1)
            {
                return BadRequest("Invalid pagination parameters or HospitalSpecialtyId.");
            }
            try
            {
                var paginationParams = new PaginationParameters { PageNumber = PageNumber, PageSize = PageSize, SearchTerm = SearchTerm };
                var result = await _doctorService.GetAllDoctorsPerHospitalSpecialty(hospitalSpecialtyId, paginationParams);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving doctors for the specialty.");
            }
        }

        [HttpGet("specialty/doctors/{specialtyId}")]
        public async Task<IActionResult> GetDoctorsBySpecialtyIdForAdmin(
    int specialtyId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? searchTerm = null)
        {
            var paginationParameters = new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
            };

            var result = await _doctorService.GetDoctorsBySpecialtyIdForAdminDashboardAsync(specialtyId, paginationParameters);
            return Ok(result);
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

            var result = await _scheduleService.GetAvailableSchedulesByDoctorIdAsync(doctorId, paginationParameters);
            return Ok(result);
        }
    }
    }
