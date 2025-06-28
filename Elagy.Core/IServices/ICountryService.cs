using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elagy.Core.DTOs.Locations;
using Elagy.Core.Entities;

namespace Elagy.Core.IServices
{
    public interface ICountryService
    {
         Task<Countries_GovernatesDTO> GetAllCountriesWithGovernates();
    }
}
