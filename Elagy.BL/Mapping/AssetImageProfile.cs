using AutoMapper;
using Elagy.Core.DTOs.User;
using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Mapping
{
    public class AssetImageProfile:Profile
    {
        public AssetImageProfile()
        {
            CreateMap<HotelAssetImage, AssetImageResponseDto>()
    .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.HotelAssetId))
    .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.ImageId))
    .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageURL))
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));




            CreateMap<HospitalAssetImage, AssetImageResponseDto>()
                .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.HospitalAssetId))
                    .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.ImageId))
    .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageURL))
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));



            CreateMap<CarRentalAssetImage, AssetImageResponseDto>()
    .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.CarRentalAssetId))
    .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.ImageId))
    .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageURL))
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

        }
    }
}
