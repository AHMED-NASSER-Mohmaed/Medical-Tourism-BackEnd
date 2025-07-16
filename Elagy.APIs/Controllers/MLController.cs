using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MLController : ControllerBase
    {
        private readonly IMLTrainingService _trainingService;
        public MLController(IMLTrainingService mLTrainingService)
        {
            _trainingService = mLTrainingService;
        }


        [HttpPost("train")]
        public async Task<IActionResult> TrainModel()
        {
            try
            {
                var isTrained = await _trainingService.TrainModel();

                if (!isTrained)
                {
                    return BadRequest("Model training failed.");
                }

                return Ok(new { message = "✅ Model trained successfully!" });
            }
            catch (Exception ex)
            {
                // optional: log ex.Message
                return StatusCode(500, $"❌ Internal server error: {ex.Message}");
            }
        }
    }
}
