using AutoMapper;
using Elagy.Core.DTOs.Package;
using Elagy.Core.DTOs.SpecialtyAppointment;
using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Mapping
{
    public class BookingProfile:Profile
    {

        public BookingProfile()
        {
            // Mapping from SpecialtyAppointment to SpecialtyAppointmentResponseDTTO

            CreateMap<SpecialtyAppointment, SpecialtyAppointmentResponseDTTO>();

            // Mapping from Packae to PackageResponseDTO
            CreateMap<Package,PackageResponseDTO>();

                 
        }

    }
}
