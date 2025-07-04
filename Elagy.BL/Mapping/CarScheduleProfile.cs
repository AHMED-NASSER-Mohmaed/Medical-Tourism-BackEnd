using AutoMapper;
using Elagy.Core.DTOs.CarRentalSchedule;
using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Mapping
{
    public class CarScheduleProfile:Profile
    {
        public CarScheduleProfile()
        {
            // Mapping from CreateCarScheduleDTO to CarSchedule
            CreateMap<CreateCarScheduleDTO,CarSchedule>();

            // Mapping from CarSchedule to CarSheduleResponseDTO
            CreateMap<CarSchedule, CarSheduleResponseDTO>();
                 
        }
    }
}
