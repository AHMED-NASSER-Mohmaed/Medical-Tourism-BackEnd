using AutoMapper;
using Elagy.Core.DTOs.Locations;
using Elagy.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Mapping
{
    public class LocationProfile:Profile
    {
        public LocationProfile()
        {

            //convert from Country to Contry_GovenateMap
            
            CreateMap<IEnumerable<Country>, Countries_GovernatesDTO>().AfterMap((src, des) =>
            {
                if(src != null && src.Any())
                {
                    foreach(Country country in src)
                    {
                        Dictionary<int,GovernateDTO> GovDTO=new Dictionary<int,GovernateDTO>();

                        if(country.Governorates!=null && country.Governorates.Any())
                        {

                            foreach (var item in country.Governorates)
                            {
                                GovDTO.Add(item.Id, new GovernateDTO()
                                {
                                    GovernateId = item.Id,
                                    GovernateName = item.Name,
                                });
                            }
                        }
                        des.Data.Add(country.Id, new Country_GovernatesDTO()
                        {
                            CountryName = country.Name,
                            CountryId = country.Id,
                            Governates = GovDTO,
                        });
                    }
                } 
            });
            
        }
    }
}
