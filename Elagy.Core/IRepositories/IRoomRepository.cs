using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IRoomRepository : IGenericRepository<Room>
    {
        /// Gets all rooms for a specific hotel asset (provider).
        Task <IEnumerable<Room>> GetRoomsByHotelId(string hotelAssetId);
        /// Gets a single room by its ID, including all related details (HotelAsset, etc.).
        Task<Room?> GetRoomByIdWithDetailsAsync(int roomId);
    }
}
