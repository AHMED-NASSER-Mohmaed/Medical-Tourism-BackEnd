using Elagy.Core.DTOs.CarAppoinment;
using Elagy.Core.Entities;
namespace Elagy.Core.IServices
{
    public interface ICarAppointmentService
    {
        Task<Package> BookAppointment(Package createdPackage, createCarRentalAppoinmentDTO _);

    }
}
