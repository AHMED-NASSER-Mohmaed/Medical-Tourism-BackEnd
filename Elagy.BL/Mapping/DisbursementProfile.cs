using AutoMapper;
using Elagy.Core.DTOs.Disbursement;
using Elagy.Core.Entities;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Mapping
{
    class DisbursementProfile : Profile
    {
        public DisbursementProfile() 
        {
            CreateMap<Disbursement, DisplayDisbursement>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.DisbursementDateMonth, opt => opt.MapFrom(src => src.DisbursementDateMonth))
                .ForMember(dest => dest.GeneratedAt, opt => opt.MapFrom(src => src.GeneratedAt))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.DisbursementItems, opt => opt.MapFrom(src => src.DisbursementItems));

            CreateMap<DisbursementItem, DisplayHospitalDisbursementItems>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.Appointment, opt => opt.MapFrom(src => (src.Appointment as SpecialtyAppointment)));
                




        }
    }
}
