using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Elagy.DAL.Repositories
{
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(ApplicationDbContext context) : base(context){}

        private IQueryable<Room> GetRoomsWithAllDetails()
        {
            return _dbSet
                .Include(r => r.HotelAsset)
                .Include(s => s.RoomImages);
        }

        public IQueryable<Room> GetRoomsByHotelId(string hotelAssetId)
        {
            var query = GetRoomsWithAllDetails()
                .Where(r => r.HotelAssetId == hotelAssetId);
            return query;
        }

        public async Task<Room?> GetRoomByIdWithDetailsAsync(int roomId)
        {
            return await GetRoomsWithAllDetails()
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }
    }
}
