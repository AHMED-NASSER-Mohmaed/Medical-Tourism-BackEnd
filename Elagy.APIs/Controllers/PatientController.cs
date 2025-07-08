using Elagy.Core.DTOs.Files;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.User;
using Elagy.Core.Entities;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Authorization; // For [Authorize] attribute
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Patient")] // Only allow users with the "Patient" role
    public class PatientController : ProfileImageBaseController
    {
        private readonly IPatientService _patientService;
        private readonly IImageProfile _pofileImageService;
        private readonly IPackgeService _packageService;
        


        public PatientController(IPatientService patientService, IImageProfile pofileImageService,ILogger<PatientController>logger,IPackgeService packge)
        :base(pofileImageService,logger)
        {
            _patientService = patientService;
            _pofileImageService = pofileImageService;
            _packageService = packge;
        }

 


        [HttpGet("profile")]
        public async Task<ActionResult<PatientDto>> GetProfile()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var profile = await _patientService.GetPatientProfileAsync(userId);
            if (profile == null)
            {
                return NotFound("Patient profile not found.");
            }
            return Ok(profile);
        }

        [HttpPut("profile")]
        public async Task<ActionResult<PatientDto>> UpdateProfile([FromBody] PatientProfileUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var updatedProfile = await _patientService.UpdatePatientProfileAsync(userId, model);
            if (updatedProfile == null)
            {
                return BadRequest("Failed to update patient profile. Check if profile exists or data is valid.");
            }
            return Ok(updatedProfile);
        }


        [HttpGet("profile/history")]
        public async Task<ActionResult> GetPackages( int pageNumber  = 1 , int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();
            try
            {
                var package = await _packageService.getPackages(userId, new PaginationParameters { PageNumber=pageNumber,PageSize=pageSize} );
                return Ok(package);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //[HttpPost("profile/history/details")]
        //public async Task<ActionResult> GetPackageDetails(Guid packageId)
        //{
        //    var userId = GetCurrentUserId();
        //    if (userId == null) return Unauthorized();
        //    if (packageId.Equals(null) || packageId.Equals(Guid.Empty))
        //    {
        //        return BadRequest("Invalid package details request.");
        //    }
        //    try
        //    {
        //        var packageDetails = await _packageService.GetPackageDetails(userId, request.PackageId);
        //        //if (packageDetails == null)
        //        //{
        //        //    return NotFound("Package details not found.");
        //        //}
        //        //return Ok(packageDetails);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}


    }
}