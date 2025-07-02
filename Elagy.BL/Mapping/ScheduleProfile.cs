using AutoMapper;
using Elagy.Core.DTOs.Schedule;
using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Mapping
{
    public class ScheduleProfile:Profile
    {
        public ScheduleProfile()
        {
            CreateMap<Core.Entities.DayOfWeek, DayOfWeekDto>()
               .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ShortCode));


            CreateMap<CreateScheduleSlotDto, Schedule>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.BookedSlots, opt => opt.MapFrom(src => 0))
                    .ForMember(dest => dest.CancelledSlots, opt => opt.MapFrom(src => 0))
                    .ForMember(dest => dest.TimeSlotSize, opt => opt.MapFrom(src => src.TimeSlotSize))
                    .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                    .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
                    .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                    .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
                    .ForMember(dest => dest.MaxCapacity, opt => opt.MapFrom(src => src.MaxCapacity))
                    .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                    .ForMember(dest => dest.HospitalSpecialtyId, opt => opt.MapFrom(src => src.HospitalSpecialtyId))
                    .ForMember(dest => dest.DayOfWeekId, opt => opt.MapFrom(src => src.DayOfWeekId));



            CreateMap<UpdateScheduleDto, Schedule>()
              .ForMember(dest => dest.Id, opt => opt.Ignore())
              .ForMember(dest => dest.BookedSlots, opt => opt.Ignore()) 
              .ForMember(dest => dest.CancelledSlots, opt => opt.Ignore()) 
              .ForMember(dest => dest.DoctorId, opt => opt.Ignore())
              .ForMember(dest => dest.HospitalSpecialtyId, opt => opt.Ignore())
              .ForMember(dest => dest.Doctor, opt => opt.Ignore())
              .ForMember(dest => dest.HospitalSpecialty, opt => opt.Ignore())
              .ForMember(dest => dest.DayOfWeek, opt => opt.Ignore());

            CreateMap<Schedule, ScheduleResponseDto>()
               .ForMember(dest => dest.DoctorId, opt => opt.MapFrom(src => src.Doctor.Id))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
               .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
               .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime))
               .ForMember(dest => dest.TimeSlotSize, opt => opt.MapFrom(src => src.TimeSlotSize))
               .ForMember(dest => dest.MaxCapacity, opt => opt.MapFrom(src => src.MaxCapacity))
               .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.FirstName + " " + src.Doctor.LastName))
               .ForMember(dest => dest.HospitalSpecialtyId, opt => opt.MapFrom(src => src.HospitalSpecialtyId))
               .ForMember(dest => dest.Hospital, opt => opt.MapFrom(src => src.HospitalSpecialty.HospitalAsset.Name))
               .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.HospitalSpecialty.Specialty.Name))
               .ForMember(dest => dest.HospitalAssetId, opt => opt.MapFrom(src => src.HospitalSpecialty.HospitalAssetId))
               .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek));
        }
    }
}
