using Elagy.Core.Entities;
using Elagy.Core.IServices;
using System.Threading.Tasks;
using DayOfWeek = Elagy.Core.Entities.DayOfWeek;

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
        /// <summary>
        /// ///////////////////////////////
        /// </summary>
        IGenericRepository<Country> Countries { get; }
        IGenericRepository<Governorate> Governates { get; }
        IGenericRepository<DayOfWeek> DayOfWeeks { get; }
        ISpecialtyRepository Specialties { get; }
        IDoctorRepository Doctors { get; }
        IHospitalRepository Hospitals { get; }
        IHospitalSpecialtyRepository HospitalSpecialties { get; }
        IScheduleRepository Schedules { get; }
        IRoomRepository Rooms { get; }

        ICarRepository Cars { get; }
        IDriverRepository Drivers { get; }
        ICarDriverRepository CarDrivers { get; }
        ///////////////////////////
        
        //nasser
        IGenericRepository<RoomSchedule> RoomSchedule { get; }
        public IGenericRepository<CarSchedule> CarSchedule { get; }

        Task<int> CompleteAsync();
    }
}