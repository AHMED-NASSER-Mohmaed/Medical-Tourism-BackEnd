using AutoMapper;
using Elagy.Core.DTOs.Room;
using Elagy.Core.DTOs.RoomAppoinment;
using Elagy.Core.DTOs.RoomSchedule;
using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Mapping
{
    public class HotelProfile:Profile
    {
        public HotelProfile()
        {
            // RoomImage to RoomImageDto
            CreateMap<RoomImage, RoomImageDto>();

            // RoomCreateDto to Room Entity
            CreateMap<RoomCreateDto, Room>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.RoomImages, opt => opt.Ignore())
                .ForMember(dest => dest.HotelAsset, opt => opt.Ignore()); 

            // RoomUpdateDto to Room Entity
            CreateMap<RoomUpdateDto, Room>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.HotelAsset, opt => opt.Ignore()) 
                .ForMember(dest => dest.RoomImages, opt => opt.Ignore()); 

            // Room Entity to RoomResponseDto
            CreateMap<Room, RoomResponseDto>()
                .ForMember(dest => dest.HotelAssetName, opt => opt.MapFrom(src => src.HotelAsset.Name))
                .ForMember(dest => dest.HotelStarRating, opt => opt.MapFrom(src => src.HotelAsset.StarRating))
                .ForMember(dest => dest.RoomImages, opt => opt.MapFrom(src => src.RoomImages));

            CreateMap<CreateRoomScheduleDTO, RoomSchedule>();

            // RoomSchedule to CreateRoomScheduleResponseDTO
            CreateMap<RoomSchedule, RoomScheduleResponseDTO>();




            // RoomAppointment to RoomAppointmentResponseDTO
            CreateMap<RoomAppointment, RoomAppointmentResponseDTO>();
                

        }
    }
}
