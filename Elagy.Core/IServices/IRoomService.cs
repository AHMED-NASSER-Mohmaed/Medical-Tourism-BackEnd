using Elagy.Core.DTOs.Pagination;
using Elagy.Core.DTOs.Room;
using Elagy.Core.DTOs.TOP;
using Elagy.Core.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IServices
{
    public interface IRoomService
    {

        //Retrieves a paginated list for admin dashboard.
        Task<PagedResponseDto<RoomResponseDto>> GetAllRoomsForHotelAdminAsync(string hotelAssetId, PaginationParameters paginationParameters);
        Task<RoomResponseDto> CreateRoomAsync(RoomCreateDto createDto, string hotelAssetId, List<IFormFile>? imageFiles = null);
        Task<RoomResponseDto> UpdateRoomAsync(int roomId, RoomUpdateDto updateDto, string hotelAssetId, List<IFormFile>? newImageFiles = null);
        //Changes the general operational availability status (IsAvailable) of a room.
        Task<RoomResponseDto> ChangeRoomAvailabilityStatusAsync(int roomId, bool newIsAvailableStatus, string hotelAssetId);
        // Changes the internal status of a specific room.
        Task<RoomResponseDto> ChangeRoomInternalStatusAsync(int roomId, RoomStatus newInternalStatus, string hotelAssetId);
        Task<PagedResponseDto<RoomResponseDto>> GetAvailableRoomsForWebsiteAsync(PaginationParameters paginationParameters, string hotellId);
        Task<RoomResponseDto?> GetRoomByIdAsync(int roomId);
        Task<IEnumerable<HotelBookingCountDto>> GetTopHotelsByBookings();


    }
}
