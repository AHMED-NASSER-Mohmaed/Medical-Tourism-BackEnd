using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Shared;
using Elagy.Core.DTOs.User;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IHotelProviderService
    {
        // Profile management
        Task<HotelProviderProfileDto> GetHotelProviderProfileAsync(string providerId);
        Task<HotelProviderProfileDto> UpdateHotelProviderProfileAsync(string providerId, HotelProviderProfileUpdateDto model);

        // For Super Admin to add providers directly
        Task<AuthResultDto> AddHotelProviderByAdminAsync(HotelProviderRegistrationRequestDto model);
    }
}