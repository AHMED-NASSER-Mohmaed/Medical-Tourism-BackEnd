using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using Elagy.Core.IServices;
using Elagy.Core.Enums; // For UserStatus, VerificationStatus
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elagy.Core.DTOs.Pagination;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")] // Only allow users with the "SuperAdmin" role
    public class SuperAdminController : BaseApiController
    {
        private readonly ISuperAdminService _superAdminService;
        private readonly IPatientService _patientService; // For admin-adding patients
        private readonly IHotelProviderService _hotelProviderService; // For admin-adding providers
        private readonly IHospitalProviderService _hospitalProviderService;
        private readonly ICarRentalProviderService _carRentalProviderService;

        public SuperAdminController(ISuperAdminService superAdminService,
                                    IPatientService patientService,
                                    IHotelProviderService hotelProviderService,
                                    IHospitalProviderService hospitalProviderService,
                                    ICarRentalProviderService carRentalProviderService
                                     )
        {
            _superAdminService = superAdminService;
            _patientService = patientService;
            _hotelProviderService = hotelProviderService;
            _hospitalProviderService = hospitalProviderService;
            _carRentalProviderService = carRentalProviderService;
        }



        // --- Super Admin Profile ---
        [HttpGet("profile")]
        public async Task<ActionResult<SuperAdminDto>> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var profile = await _superAdminService.GetSuperAdminProfileAsync(userId);
            if (profile == null)
            {
                return NotFound("Super Admin profile not found.");
            }
            return Ok(profile);
        }




        #region  --- Admin Add Functionality (delegating to specific services) ---


        [HttpPost("add-patient")]
        public async Task<ActionResult> AddPatientByAdmin([FromBody] PatientRegistrationRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _patientService.AddPatientByAdminAsync(model);
            return HandleAuthResult(result);
        }

        [HttpPost("add-hotel-provider")]
        public async Task<ActionResult> AddHotelProviderByAdmin([FromBody] HotelAssetRegistrationRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _hotelProviderService.AddHotelProviderByAdminAsync(model);
            return HandleAuthResult(result);
        }

        [HttpPost("add-hospital-provider")]
        public async Task<ActionResult> AddHospitalProviderByAdmin([FromBody] HospitalAssetRegistrationRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _hospitalProviderService.AddHospitalProviderByAdminAsync(model);
            return HandleAuthResult(result);
        }

        [HttpPost("add-car-rental-provider")]
        public async Task<ActionResult> AddCarRentalProviderByAdmin([FromBody] CarRentalAssetRegistrationRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _carRentalProviderService.AddCarRentalProviderByAdminAsync(model);
            return HandleAuthResult(result);
        }

        #endregion



        #region --- User Status Management ---
        [HttpPut("activate-user/{userId}")]
        public async Task<ActionResult> ActivateUser(string userId)
        {
            var result = await _superAdminService.ActivateUserAccountAsync(userId);
            return HandleAuthResult(result);
        }

        [HttpPut("deactivate-user/{userId}")]
        public async Task<ActionResult> DeactivateUser(string userId)
        {
            var result = await _superAdminService.DeactivateUserAccountAsync(userId);
            return HandleAuthResult(result);
        }

        [HttpPut("approve-provider/{providerId}")]
        public async Task<ActionResult> ApproveProvider(string providerId)
        {
            var result = await _superAdminService.ApproveServiceProviderAccountAsync(providerId);
            return HandleAuthResult(result);
        }

        [HttpPut("reject-provider/{providerId}")]
        public async Task<ActionResult> RejectProvider(string providerId, [FromBody] string rejectionReason)
        {
            var result = await _superAdminService.RejectServiceProviderAccountAsync(providerId, rejectionReason);
            return HandleAuthResult(result);
        }

        #endregion


        /*    // --- Profile Details Retrieval for Admin Dashboard Modals ---
        [HttpGet("patients/{patientId}")]
        public async Task<ActionResult<PatientDto>> GetPatientDetails(string patientId)
        {
            var profile = await _superAdminService.GetPatientDetailsForAdminAsync(patientId);
            if (profile == null) return NotFound("Patient not found.");
            return Ok(profile);
        }

        [HttpGet("hotel-providers/{providerId}")]
        public async Task<ActionResult<HotelProviderProfileDto>> GetHotelProviderDetails(string providerId)
        {
            var profile = await _superAdminService.GetHotelProviderDetailsForAdminAsync(providerId);
            if (profile == null) return NotFound("Hotel Provider not found.");
            return Ok(profile);
        }

*/


        #region --- Listing/Filtering for Dashboard Tabs ---
        [HttpGet("patients")]
        public async Task<ActionResult< PagedResponseDto<PatientDto>>> GetPatients(
            [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10,
            [FromQuery] string SearchTerm = null, [FromQuery] Status? UserStatus = null)
        {
            var Filter = new PaginationParameters();
            Filter.PageNumber = PageNumber ;
            Filter.PageSize = PageSize;
            Filter.SearchTerm = SearchTerm;
            Filter.UserStatus = UserStatus;

            var patients = await _superAdminService.GetPatientsForAdminDashboardAsync(Filter);
            return Ok(patients);
        }

        [HttpGet("hotel-providers")]
        public async Task<ActionResult<IEnumerable<HotelProviderProfileDto>>> GetHotelProviders(
            [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10,
            [FromQuery] string SearchTerm = null, [FromQuery] Status? UserStatus = null)
        {
            var Filter = new PaginationParameters();
            Filter.PageNumber = PageNumber;
            Filter.PageSize = PageSize;
            Filter.SearchTerm = SearchTerm;
            Filter.UserStatus = UserStatus;

            var providers = await _superAdminService.GetHotelProvidersForAdminDashboardAsync(Filter);
            return Ok(providers);
        }


        [HttpGet("hospital-providers")]
        public async Task<ActionResult<IEnumerable<HotelProviderProfileDto>>> GetHospitalProviders(
        [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10,
        [FromQuery] string SearchTerm = null, [FromQuery] Status? UserStatus = null,
        [FromQuery] int? specialtyId=null)
        {
            var Filter = new PaginationParameters();
            Filter.PageNumber = PageNumber;
            Filter.PageSize = PageSize;
            Filter.SearchTerm = SearchTerm;
            Filter.UserStatus = UserStatus;
            Filter.SpecialtyId= specialtyId;

            var providers = await _superAdminService.GetHospitalProvidersForAdminDashboardAsync(Filter);
            return Ok(providers);
        }


        [HttpGet("CarRental-providers")]
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

        #endregion --- End Of Listing/Filtering for Dashboard Tabs ---








        #region --- Admin Initiated Email/Password Changes ---
        [HttpPut("change-user-email")]
        public async Task<ActionResult> AdminChangeUserEmail(string userId, [FromBody] string newEmail)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState); // Check for newEmail string format if complex
            var result = await _superAdminService.AdminChangeUserEmailAsync(userId, newEmail);
            return HandleAuthResult(result);
        }


        [HttpPost("reset-user-password/{userId}")]
        public async Task<ActionResult> AdminResetUserPassword(string userId)
        {
            var result = await _superAdminService.AdminResetUserPasswordAsync(userId);
            return HandleAuthResult(result);
        }
        
        #endregion




        /*        [HttpPut("set-asset-verification-status/{assetId}")]
                public async Task<ActionResult> SetAssetVerificationStatus(string assetId, [FromQuery] VerificationStatus status, [FromBody] string notes = null)
                {
                    var result = await _superAdminService.AdminSetAssetVerificationStatusAsync(assetId, status, notes);
                    return HandleAuthResult(result);
                }
        */






    }
}