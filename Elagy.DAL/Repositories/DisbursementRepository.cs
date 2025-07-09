using Elagy.Core.Entities;
using Elagy.Core.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.DAL.Repositories
{
    public class DisbursementRepository : GenericRepository<Disbursement> , IDisbursementRepository
    {
        public DisbursementRepository(ApplicationDbContext _context):base(_context)
        { }
        
        public async Task<IQueryable<Disbursement>> GetAllHospitalDisbursement(string AssetId)
        {
            Console.WriteLine("inside Repo");
            
            return  _dbSet.Where(d=> d.AssetId==AssetId).Select(d => new Disbursement
            {
                Id = d.Id,
                DisbursementDateMonth = d.DisbursementDateMonth,
                TotalAmount = d.TotalAmount,
                GeneratedAt = d.GeneratedAt,
                PaymentMethod = d.PaymentMethod,
                 

            });
        }

       

        public async Task<Disbursement> GetHospitalDisbursementById(int disbursementId)
        {
            return await _dbSet.Include(d => d.DisbursementItems)
                        .ThenInclude(item => item.Appointment )
                            .ThenInclude(a => (a as SpecialtyAppointment).SpecialtySchedule)
                    .FirstOrDefaultAsync(d => d.Id == disbursementId);
        }
        public Task<IQueryable<Disbursement>> GetAllHotelDisbursement(string AssetId)
        {
            throw new NotImplementedException();
        }

        public Task<Disbursement> GetHotelDisbursementById(int disbursementId)
        {
            throw new NotImplementedException();
        }

        public Task<IQueryable<Disbursement>> GetAllCarRentalDisbursement(string AssetId)
        {
            throw new NotImplementedException();
        }

        public Task<Disbursement> GetCarRentalDisbursementById(int disbursementId)
        {
            throw new NotImplementedException();
        }
    }
}
