using Elagy.Core.DTOs.Locations;
using Elagy.Core.Entities;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;

namespace Elagy.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet("Countries-Governates")]
        public async Task<ActionResult<Country_GovernatesDTO>> GetCountriesWithGovernates()
        {
            try
            {
                var result = await _countryService.GetAllCountriesWithGovernates();

                if (result == null || !result.Data.Any())
                    return NotFound("No countries with governorates found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception if needed, or return a generic error
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
