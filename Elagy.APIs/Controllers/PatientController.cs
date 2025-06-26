using Elagy.Core.DTOs.Files;
using Elagy.Core.DTOs.User;
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
 


        public PatientController(IPatientService patientService, IImageProfile pofileImageService,ILogger<PatientController>logger)
        :base(pofileImageService,logger)
        {
            _patientService = patientService;
            _pofileImageService = pofileImageService;
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
    
    
    
    
    }
}