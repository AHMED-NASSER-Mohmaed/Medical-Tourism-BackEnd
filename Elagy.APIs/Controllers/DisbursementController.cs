using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisbursementController : BaseApiController
    {

        public DisbursementController() 
        {
            // Constructor logic if needed
        }

        [HttpGet]
        
        public IActionResult GetDisbursements()
        {
            try
            {

                var userId = GetCurrentUserId();
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated.");
                }

                return Ok("List of disbursements");
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
            
        }
    }
}
