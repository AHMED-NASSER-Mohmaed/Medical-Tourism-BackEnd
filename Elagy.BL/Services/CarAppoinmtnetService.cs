using Elagy.Core.DTOs.CarAppoinment;
using Elagy.Core.DTOs.CarRentalSchedule;
using Elagy.Core.Entities;
using Elagy.Core.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class CarAppoinmtnetService : ICarAppointmentService
    {
        private readonly ICarScheduleService _carScheduleService;

        public CarAppoinmtnetService(ICarScheduleService carSchedule)
        {
            _carScheduleService = carSchedule;
        }
        public async Task<Package> BookAppointment(Package createdPackage, createCarRentalAppoinmentDTO CRADTO)
        {


            if (createdPackage == null)
            {
                throw new ArgumentNullException(nameof(createdPackage), "Package cannot be null");
            }

            CarSheduleResponseDTO cretedSchedue =await _carScheduleService.CreateCarSchedule(new CreateCarScheduleDTO
            {
                CarId = CRADTO.CarId,
                StartingDate = CRADTO.StartingDate,
                EndingDate = CRADTO.EndingDate,
            });


            //here you can add logic to save the appointment details to the database 

            //var CarAppointment = new CarRentalAppointment
            //{
            //    PackageId = createdPackage.Id, // Assuming the package has an Id property
            //    StartingDate = CRADTO.StartingDate,
            //    EndingDate = CRADTO.EndingDate,
            //    price = ,
            //};



            throw new NotImplementedException();
        }
    }
}
