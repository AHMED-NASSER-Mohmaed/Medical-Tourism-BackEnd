using Elagy.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IDisbursementRepository : IGenericRepository<Disbursement>
    {
        Task<IQueryable<Disbursement>> GetAllHospitalDisbursement(string AssetId);
        Task<Disbursement> GetHospitalDisbursementById(int disbursementId);


        Task<IQueryable<Disbursement>> GetAllHotelDisbursement(string AssetId);
        Task<Disbursement> GetHotelDisbursementById(int disbursementId);

        Task<IQueryable<Disbursement>> GetAllCarRentalDisbursement(string AssetId);
        Task<Disbursement> GetCarRentalDisbursementById(int disbursementId);
    }
}
