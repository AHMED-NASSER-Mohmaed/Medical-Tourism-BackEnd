using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<IEnumerable<Room>> GetRoomsByHotelIdAsync(string hotelAssetId, bool? isAvailable = null, RoomStatus? status = null)
        {
            var query = GetRoomsWithAllDetails()
                .Where(r => r.HotelAssetId == hotelAssetId);

            if (isAvailable.HasValue)
            {
                query = query.Where(r => r.IsAvailable == isAvailable.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(r => r.Status == status.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<Room?> GetRoomByIdWithDetailsAsync(int roomId)
        {
            return await GetRoomsWithAllDetails()
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }
    }
}
