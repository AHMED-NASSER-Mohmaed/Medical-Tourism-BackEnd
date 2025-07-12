
using Elagy.Core.DTOs.CarlSchedule;
using Elagy.Core.DTOs.CarRentalSchedule;




namespace Elagy.Core.IServices
{
    public interface ICarScheduleService
    {
        Task<bool> IsAvilable(DateOnly Start, DateOnly End, int CarId);
        Task<CarSheduleResponseDTO> CreateCarSchedule(CreateCarScheduleDTO carScheduleDTO);

        Task<CarUnavailableDatesDTO> GetAvailableCarsSchedules(int carId);

    }
}
