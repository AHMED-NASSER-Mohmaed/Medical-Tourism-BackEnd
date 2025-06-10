using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface ICarRentalProviderService
    {
        Task<CarRentalProviderProfileDto> GetCarRentalProviderProfileAsync(string providerId);
        Task<CarRentalProviderProfileDto> UpdateCarRentalProviderProfileAsync(string providerId, CarRentalProviderProfileUpdateDto model);
        Task<AuthResultDto> AddCarRentalProviderByAdminAsync(CarRentalProviderRegistrationRequestDto model);
    }
}