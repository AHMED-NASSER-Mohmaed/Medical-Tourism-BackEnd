using Elagy.Core.Entities;
using System.Threading.Tasks;

namespace Elagy.Core.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        // Expose concrete repositories for all our concrete entities
        IGenericRepository<Patient> Patients { get; }
        IGenericRepository<ServiceProvider> ServiceProviders { get; }
        IGenericRepository<SuperAdmin> SuperAdmins { get; }

        IGenericRepository<Asset> ServiceAssets { get; } // Access to base ServiceAsset for queries
        IGenericRepository<HotelAsset> HotelAssets { get; }
        IGenericRepository<HospitalAsset> HospitalAssets { get; }
        IGenericRepository<CarRentalAsset> CarRentalAssets { get; }



        Task<int> CompleteAsync();
    }
}