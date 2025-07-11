using AutoMapper;
using Elagy.Core.DTOs.Booking;
 using Elagy.Core.DTOs.CarRentals;
using Elagy.Core.DTOs.Driver;
using Elagy.Core.DTOs.Package;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.BL.Services
{
    public class PackageService : IPackgeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _map  ;
        public PackageService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _map = mapper;
        }

        public async Task<PackageResponseDTO> CancelBooking(string packageId)
        {
            var Pakage = await _unitOfWork.Packages.AsQueryable()
                .Where(p => p.Id == Guid.Parse(packageId))
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync();
            if (Pakage == null)
            {
                throw new ArgumentException("Package not found. Invalid Package Id", nameof(packageId));
            }
            
            if (Pakage.Status == BookingStatus.Confirmed)
            {
                foreach (var appointment in Pakage.Appointments)
            {
                if (appointment is SpecialtyAppointment specialtyAppointment)
                {
                    if (specialtyAppointment.Date < DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1))
                    {
                        throw new ArgumentException("Cannot cancel a past appointment.", nameof(packageId));
                    }

                }
                else if (appointment is RoomAppointment roomAppointment)
                {
                    if (roomAppointment.CheckInDate < DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1))
                    {
                        throw new ArgumentException("Cannot cancel a past appointment.", nameof(packageId));
                    }
                }
                else if (appointment is CarRentalAppointment carAppointment)
                {
                    if (carAppointment.StartingDate < DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1))
                    {
                        throw new ArgumentException("Cannot cancel a past appointment.", nameof(packageId));
                    }
                }
            }

                Pakage.Status = BookingStatus.Canceled;
                foreach (var a in Pakage.Appointments)
                {
                    a.Status = AppointmentStatus.Cancelled;
                }

                _unitOfWork.Packages.Update(Pakage);
                await _unitOfWork.CompleteAsync();
            }
            else
            {
                throw new ArgumentException("Package is not in a cancellable state.", nameof(packageId));
            }


            return _map.Map<PackageResponseDTO>(Pakage);
        }

        public async Task<Package> CreatePackage(string PatientId)
        {
            if (string.IsNullOrEmpty(PatientId))
            {
                throw new ArgumentException("Patient ID cannot be null or empty.", nameof(PatientId));
            }

            var createdPackage = new Package
            {
                PatientId = PatientId,
                CreatedAt = DateTime.UtcNow,
                //Status = BookingStatus.PendingPayment,
                Status = BookingStatus.Confirmed,

            };

            await _unitOfWork.Packages.AddAsync(createdPackage);


            return createdPackage;
        }

        public async Task<PackageWithDetailDTO> GetPackageDetails(Guid packageId)
        {
            if (packageId == Guid.Empty)
            {
                throw new ArgumentException("Package ID cannot be empty.", nameof(packageId));
            }

            var package = await _unitOfWork.Packages.AsQueryable()
                .Where(p => p.Id == packageId)
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync();


            if (package == null)
            {
                throw new ArgumentException("Package not found.Invalid Package Id", nameof(packageId));
            }


            PackageDetailsDTO DetailsOfThePackage = new();

            var PacakgeDTO = _map.Map<PackageResponseDTO>(package);


            /********************************************/

            var SpeialityQuery = _unitOfWork.SpecialtySchedule.AsQueryable()
                .Where(s =>  PacakgeDTO.SpecialtyAppoinment.SpecialtyScheduleId == s.Id)
                .Include(s=>s.HospitalSpecialty)
                .ThenInclude(HospitalSpecialty => HospitalSpecialty.Specialty)
                .Include(s => s.Doctor)
                .ThenInclude(HS => HS.HospitalSpecialty.HospitalAsset)
                .Select(s => new 
                {
                    SepcialtyScheduleId = s.Id,
                    SepcialtyName = s.HospitalSpecialty.Specialty.Name,

                    DoctorId = s.Doctor.Id,
                    DoctorFName = s.Doctor.FirstName,
                    DoctorLName = s.Doctor.LastName,
                    DoctorIamgeUrl = s.Doctor.ImageURL,

                    HospitalName = s.Doctor.HospitalSpecialty.HospitalAsset.Name,
                    HospitalGovId = s.Doctor.HospitalSpecialty.HospitalAsset.GovernateId,

                });

            var ResultOfSpecialty = await SpeialityQuery.FirstOrDefaultAsync();


            var returnObj = new PackageWithDetailDTO
            {
                DoctorName = ResultOfSpecialty.DoctorFName + " " + ResultOfSpecialty.DoctorLName,
                DoctorImageUrl = ResultOfSpecialty.DoctorIamgeUrl,
                DoctorSpecialtyName = ResultOfSpecialty.SepcialtyName,
                HospitalName = ResultOfSpecialty.HospitalName,
                HospitalGovId = ResultOfSpecialty.HospitalGovId,
                AppointmentDate = PacakgeDTO.SpecialtyAppoinment.Date,
                AppointmentExistingTime = PacakgeDTO.SpecialtyAppoinment.ExistingTime,
                AppointmentPrice = PacakgeDTO.SpecialtyAppoinment.price,
                
            };

           

            if (PacakgeDTO.RoomAppointment != null)
            {
               var  RoomQuery = _unitOfWork.RoomSchedule.AsQueryable()
                    .Where(r => PacakgeDTO.RoomAppointment.HotelScheduleId == r.Id)
                    .Include(rs => rs.Room)
                    .ThenInclude(r => r.HotelAsset)
                    .Include(r => r.Room.RoomImages)
                    .Select(r => new
                    {
                        RoomId = r.Room.Id,
                        RoomType = r.Room.RoomType,
                        RoomImageUrl = r.Room.RoomImages != null ? r.Room.RoomImages.Select(i => i.ImageURL).FirstOrDefault() : null,
                        RoomViewType = r.Room.ViewType,
                        HotelAssetName = r.Room.HotelAsset.Name,
                        GovernorateId = r.Room.HotelAsset.GovernateId,
                    });

                var ResultOfRoom = await RoomQuery.FirstOrDefaultAsync() ;

                returnObj.HospitalGovId = ResultOfRoom?.GovernorateId ?? 0;
                returnObj.HotelName = ResultOfRoom?.HotelAssetName ?? string.Empty;
                returnObj.RoomType = ResultOfRoom?.RoomType?? 0;
                returnObj.RoomImageUrl = ResultOfRoom?.RoomImageUrl ?? string.Empty;
                returnObj.HotelPrice = PacakgeDTO.RoomAppointment.price;
                returnObj.RoomView = ResultOfRoom?.RoomViewType ??  0 ;
                returnObj.CheckInDate = PacakgeDTO.RoomAppointment.CheckInDate;
                returnObj.CheckOutDate = PacakgeDTO.RoomAppointment.CheckOutDate;
                returnObj.HotelPrice = PacakgeDTO.RoomAppointment.price;
            }


            if (PacakgeDTO.CarAppointment != null)
            {
                /* var CarQuery = _unitOfWork.CarSchedule.AsQueryable()
                             .Where(cs => cs.Id == PacakgeDTO.CarAppointment.CarScheduleId)
                             .Join(
                                 _unitOfWork.Cars.AsQueryable(),
                                 cs => cs.CarId,
                                 car => car.Id,
                                 (cs, car) => new { cs, car }
                             )
                             .Join(
                                 _unitOfWork.CarRentalAssets.AsQueryable(),
                                 temp => temp.car.CarRentalAssetId,
                                 asset => asset.Id,
                                 (temp, asset) => new { temp.cs, temp.car, asset }
                             )
                             .GroupJoin(
                                 _unitOfWork.CarDrivers.AsQueryable().Where(cd => cd.IsAssignedCurrent),
                                 temp => temp.car.Id,
                                 driver => driver.CarId,
                                 (temp, drivers) => new
                                 {
                                     temp.cs,
                                     temp.car,
                                     temp.asset,
                                     currentDriver = drivers.FirstOrDefault()
                                 }
                             )
                             .GroupJoin(
                                 _unitOfWork.CarImages.AsQueryable(),
                                 temp => temp.car.Id,
                                 CI => CI.CarId,
                                 (temp, CI) =>new
                                 {
                                     temp.cs,
                                     temp.car,
                                     temp.asset,
                                     temp.currentDriver,
                                     carImage = CI.FirstOrDefault()
                                 }
                             )
                             .Select(x=> new
                             {
                                  CarImageURL= x.carImage != null ? x.carImage.ImageURL : string.Empty,
                                  CarModelName = x.car.ModelName,
                                  CarRentalAssetName = x.asset.Name,
                                  DriverId = x.currentDriver != null ? x.currentDriver.DriverId : string.Empty,
                                  DriverImageURl= x.currentDriver != null ? x.currentDriver.Driver.ImageURL : string.Empty,
                                  DriverName=x.currentDriver != null ? x.currentDriver.Driver.FirstName + " " + x.currentDriver.Driver.LastName : string.Empty

                             });



                 var ResultOfCar =  await CarQuery.FirstOrDefaultAsync();*/

                var carQuery =
                from cs in _unitOfWork.CarSchedule.AsQueryable()
                where cs.Id == PacakgeDTO.CarAppointment.CarScheduleId

                join car in _unitOfWork.Cars.AsQueryable() on cs.CarId equals car.Id
                join asset in _unitOfWork.CarRentalAssets.AsQueryable() on car.CarRentalAssetId equals asset.Id

                join driver in _unitOfWork.CarDrivers.AsQueryable().Where(cd => cd.IsAssignedCurrent)
                    on car.Id equals driver.CarId into driverGroup
                from currentDriver in driverGroup.DefaultIfEmpty()

                join img in _unitOfWork.CarImages.AsQueryable()
                    on car.Id equals img.CarId into imageGroup
                from carImage in imageGroup.DefaultIfEmpty()

                select new
                {
                    CarImageURL = carImage != null ? carImage.ImageURL : string.Empty,
                    CarModelName = car.ModelName,
                    CarRentalAssetName = asset.Name,
                    DriverId = currentDriver != null ? currentDriver.DriverId : string.Empty,
                    DriverImageURl = currentDriver != null ? currentDriver.Driver.ImageURL : string.Empty,
                    DriverName = currentDriver != null
                        ? currentDriver.Driver.FirstName + " " + currentDriver.Driver.LastName
                        : string.Empty
                };

                var ResultOfCar = await carQuery.FirstOrDefaultAsync();


                returnObj.CarModel = ResultOfCar?.CarModelName ?? string.Empty;
                returnObj.CarImageUrl = ResultOfCar.CarImageURL;
                returnObj.rentalCompanyName = ResultOfCar?.CarRentalAssetName ?? string.Empty;
                returnObj.CarPrice = PacakgeDTO.CarAppointment.price;
                returnObj.DriverId = ResultOfCar.DriverId;
                returnObj.DriverName = ResultOfCar?.DriverName ?? string.Empty;
                returnObj.DriverImageUrl = ResultOfCar?.DriverImageURl ?? string.Empty;

            }
            return returnObj;
        }


        public async Task<PagedResponseDto<PackageResponseDTO>> getPackages(string _patientId,PaginationParameters pp)
        {
            if (string.IsNullOrEmpty(_patientId))
            {
                throw new ArgumentException("Patient ID cannot be null or empty.", nameof(_patientId));
            }

            var query=_unitOfWork.Packages.AsQueryable()
                .Where(p => p.PatientId == _patientId)
                .Include(p => p.Appointments);

            int TotalCount = await query.CountAsync();
            var SkippedPages = pp.PageSize * (pp.PageNumber - 1);

            var packageQuery = query.Skip(SkippedPages)
                .Take(pp.PageSize);
                 

            var packageList = await packageQuery.ToListAsync();


            if (packageList == null || !packageList.Any())
            {
                return new PagedResponseDto<PackageResponseDTO>(
                    new List<PackageResponseDTO>(),
                    0,
                    pp.PageNumber,
                    pp.PageSize);
            }

            var PackageResponseDTOList =_map.Map<List<PackageResponseDTO>>(packageList);


 

            return new PagedResponseDto<PackageResponseDTO>(
            PackageResponseDTOList,
            TotalCount,
            pp.PageNumber,
            pp.PageSize
            );
             
        }
    


    
    }
}
