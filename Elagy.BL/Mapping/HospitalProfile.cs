using AutoMapper;
using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.SpecialtyDTO;
using Elagy.Core.DTOs.User;
using Elagy.Core.Entities;
using Elagy.Core.Enums; // For UserType, AssetType
using ServiceProvider = Elagy.Core.Entities.ServiceProvider; // Ensure this is the correct namespace for ServiceProvider
//due to the confustion that happent between the serviceprovider  injection service 
namespace Elagy.BL.Mapping
{
    public class HospitalProfile:Profile
    {
        public HospitalProfile()
        {

          
            CreateMap<Specialty, SpecialtyDto>().ReverseMap(); // For CRUD and nested views
            CreateMap<SpecialtyCreateDto, Specialty>();
            CreateMap<SpecialtyUpdateDto, Specialty>();


        }
    }
}