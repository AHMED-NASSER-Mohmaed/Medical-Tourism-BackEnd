using Elagy.Core.IRepositories;
using Elagy.Core.Entities;
using Elagy.DAL.Repositories;
using System;
using System.Threading.Tasks;
using Elagy.Core.IServices;
using DayOfWeek = Elagy.Core.Entities.DayOfWeek;

namespace Elagy.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<Patient> Patients { get; private set; }
        public IGenericRepository<ServiceProvider> ServiceProviders { get; private set; }
        public IGenericRepository<SuperAdmin> SuperAdmins { get; private set; }
        public IGenericRepository<Asset> ServiceAssets { get; private set; }
        public IGenericRepository<HotelAsset> HotelAssets { get; private set; }
        public IGenericRepository<HospitalAsset> HospitalAssets { get; private set; }
        public IGenericRepository<CarRentalAsset> CarRentalAssets { get; private set; }
        public IGenericRepository<Country> Countries{ get; private set; }
        public IGenericRepository<Governorate> Governates { get; private set; }
        public IGenericRepository<RoomSchedule> RoomSchedule { get; private set; }
        public IGenericRepository<CarSchedule> CarSchedule { get; private set; }



        public ISpecialtyRepository Specialties { get; private set; }

        public IDoctorRepository Doctors { get; private set; }

        public IHospitalRepository Hospitals { get; private set; }
        public IHospitalSpecialtyRepository HospitalSpecialties { get; private set; }



        public IGenericRepository<SpecialtyAppointment> SpecialtyAppointments { get; private set; }



        public IScheduleRepository SpecialtySchedule {  get; private set; }

        public IGenericRepository<Core.Entities.DayOfWeek> DayOfWeeks { get; private set; }

        public IRoomRepository Rooms { get; private set; }

        public ICarRepository Cars  { get; private set; }

        public IDriverRepository Drivers { get; private set; }

        public ICarDriverRepository CarDrivers { get; private set; }

        public IGenericRepository<Package> Packages { get; private set; }
        public IGenericRepository<RoomAppointment> RoomAppointments { get; private set; }

        public IGenericRepository<CarRentalAppointment> CarRentalAppointments { get; private set; }

        public IGenericRepository<HotelAssetImage> HotelAssetImages { get; private set; }

        public IGenericRepository<HospitalAssetImage> HospitalAssetImages { get; private set; }

        public IGenericRepository<CarRentalAssetImage> CarRentalAssetImages { get; private set; }

        public IGenericRepository<CarImage> CarImages { get; private set; }
        //public IGenericRepository<Disbursement> Disbursements { get; private set; }

        public IDisbursementRepository Disbursements {get; private set; }

        public IMLRepository MLAppointments { get; set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            //IRepo injection
            //later i am going to make it lazy init
            _context = context;
            Patients = new GenericRepository<Patient>(_context);
            ServiceProviders = new GenericRepository<ServiceProvider>(_context);
            SuperAdmins = new GenericRepository<SuperAdmin>(_context);
            ServiceAssets = new GenericRepository<Asset>(_context);
            HotelAssets = new GenericRepository<HotelAsset>(_context);
            HospitalAssets = new GenericRepository<HospitalAsset>(_context);
            CarRentalAssets = new GenericRepository<CarRentalAsset>(_context);
            Countries = new GenericRepository<Country>(_context);
            Governates = new GenericRepository<Governorate>(_context);
            CarRentalAppointments=new GenericRepository<CarRentalAppointment>(_context);
            RoomAppointments =new GenericRepository<RoomAppointment>(_context);
            DayOfWeeks=new GenericRepository<DayOfWeek>(_context);
            HospitalSpecialties = new HospitalSpecialtyRepository(_context);
            Specialties = new SpecialtyRepository(_context);
            Doctors = new DoctorRepository(_context);
            Hospitals = new HospitalRepository(_context);
            SpecialtySchedule = new ScheduleRepository(_context);
            Rooms = new RoomRepository(_context);
            Cars = new CarRepository(_context);
            Drivers =new DriverRepository(_context);
            CarDrivers = new CarDriverRepository(_context);

            RoomSchedule = new GenericRepository<RoomSchedule>(_context);

            CarSchedule = new GenericRepository<CarSchedule>(_context);

            SpecialtyAppointments = new GenericRepository<SpecialtyAppointment>(_context);

            Packages = new GenericRepository<Package>(_context);
            HotelAssetImages=new GenericRepository<HotelAssetImage>(_context);
            HospitalAssetImages=new GenericRepository<HospitalAssetImage>(_context);
            CarRentalAssetImages = new GenericRepository<CarRentalAssetImage>(_context);

            CarImages = new GenericRepository<CarImage>(_context);


            Disbursements = new DisbursementRepository(_context);
            MLAppointments = new MLRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}