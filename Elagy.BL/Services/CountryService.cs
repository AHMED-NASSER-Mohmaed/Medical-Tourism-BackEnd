using AutoMapper;
using Elagy.Core.DTOs.Locations;
using Elagy.Core.Entities;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.Extensions.Logging;


namespace Elagy.BL.Services
{
    public class CountryService : ICountryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CountryService> _logger;

        public CountryService(IUnitOfWork unitOfWork, IMapper mapper,ILogger<CountryService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Countries_GovernatesDTO> GetAllCountriesWithGovernates()
        {
            try
            {
                var countries = await _unitOfWork.Countries.GetAllAsync(c => c.Governorates);


                return _mapper.Map<Countries_GovernatesDTO>(countries) ;
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogError(ex, "ArgumentNullException occurred in GetAllCountriesWithGovernates: {Message}", ex.Message);
                return new Countries_GovernatesDTO();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred in GetAllCountriesWithGovernates: {Message}", ex.Message);
                throw;
            }
        }


    }
}
