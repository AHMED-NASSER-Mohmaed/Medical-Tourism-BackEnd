using AutoMapper;
using Elagy.Core.DTOs.Booking;
using Elagy.Core.DTOs.Package;
using Elagy.Core.DTOs.Pagination;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using Elagy.Core.IRepositories;
using Elagy.Core.IServices;
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
                var CarQuery = _unitOfWork.CarSchedule.AsQueryable()
                     .Where(c => PacakgeDTO.CarAppointment.Id == c.Id)
                     .Include(c => c.Car)
                     .ThenInclude(c => c.CarRentalAsset)
                     .Include(c => c.Car.CarDrivers.Where(c=>c.IsAssignedCurrent).FirstOrDefault())
                     .ThenInclude(c => c.Driver)
                     .Include(c => c.Car.CarImages)
                     .Select(cs => new
                     {
                         CarId = cs.Car.Id,
                         CarModelName = cs.Car.ModelName,
                         CarImageUrl = cs.Car.CarImages.Select(car_image=>car_image.ImageURL).FirstOrDefault(),
                         CarType = cs.Car.Type,
                         CarRentalAssetName = cs.Car.CarRentalAsset.Name,
                         DriverId =   cs.Car.CarDrivers.Select(CD=>CD.DriverId).FirstOrDefault(),
                         DriverFName = cs.Car.CarDrivers.Select(cd => cd.Driver != null ? cd.Driver.FirstName : null).FirstOrDefault(),
                         DriverLName =  cs.Car.CarDrivers.Where(cd => cd.IsAssignedCurrent).Select(cd => cd.Driver != null ? cd.Driver.LastName : null).FirstOrDefault() ,
                         DriverImageUrl =cs.Car.CarDrivers.Where(cd => cd.IsAssignedCurrent).Select(cd => cd.Driver != null ? cd.Driver.ImageURL : null).FirstOrDefault() 
                     });


                var ResultOfCar = CarQuery != null ? await CarQuery.FirstOrDefaultAsync() : null;

                returnObj.CarModel = ResultOfCar?.CarModelName ?? string.Empty;
                returnObj.CarImageUrl = ResultOfCar.CarImageUrl;
                returnObj.rentalCompanyName = ResultOfCar?.CarRentalAssetName ?? string.Empty;
                returnObj.CarPrice = PacakgeDTO.CarAppointment.price;
                returnObj.DriverId = ResultOfCar.DriverId;  
                returnObj.DriverName = ResultOfCar?.DriverFName + " " + ResultOfCar?.DriverLName ?? string.Empty;
                returnObj.DriverImageUrl = ResultOfCar?.DriverImageUrl ?? string.Empty;

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
