using Elagy.Core.IRepositories;
using Elagy.DAL.Data;
using Elagy.DAL.Repositories;
using System;
using System.Threading.Tasks;
using Elagy.Core.Entities;

namespace Elagy.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IGenericRepository<Patient> Patients { get; private set; }
        public IGenericRepository<ServiceProvider> ServiceProviders { get; private set; }
        public IGenericRepository<SuperAdmin> SuperAdmins { get; private set; }
        public IGenericRepository<ServiceAsset> ServiceAssets { get; private set; }
        public IGenericRepository<HotelAsset> HotelAssets { get; private set; }
        public IGenericRepository<HospitalAsset> HospitalAssets { get; private set; }
        public IGenericRepository<CarRentalAsset> CarRentalAssets { get; private set; }

<<<<<<< HEAD
        public ISpecialtyRepository Specialties { get; private set; }

        public IDoctorRepository Doctors { get; private set; }

        public IHospitalRepository Hospitals { get; private set; }
        public IHospitalSpecialtyRepository HospitalSpecialties { get; private set; }
=======
        // NEW: Repository for temporary ImageKit files
        public IGenericRepository<ImageKitTempFile> ImageKitTempFiles { get; private set; }
>>>>>>> main

        public UnitOfWork(ApplicationDbContext context)
        {
            //IRepo injection
            _context = context;
            Patients = new GenericRepository<Patient>(_context);
            ServiceProviders = new GenericRepository<ServiceProvider>(_context);
            SuperAdmins = new GenericRepository<SuperAdmin>(_context);
            ServiceAssets = new GenericRepository<ServiceAsset>(_context);
            HotelAssets = new GenericRepository<HotelAsset>(_context); 
            HospitalAssets = new GenericRepository<HospitalAsset>(_context);
            CarRentalAssets = new GenericRepository<CarRentalAsset>(_context);
<<<<<<< HEAD
            //////////////////
            Specialties = new SpecialtyRepository(_context);
            Doctors = new DoctorRepository(_context);
            Hospitals = new HospitalRepository(_context);
            HospitalSpecialties = new HospitalSpecialtyRepository(_context);

=======

             ImageKitTempFiles = new GenericRepository<ImageKitTempFile>(_context);
>>>>>>> main
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