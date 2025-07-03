//using AutoMapper;
//using Elagy.Core.DTOs.CarRentals;
//using Elagy.Core.DTOs.Driver;
//using Elagy.Core.DTOs.Room;
//using Elagy.Core.DTOs.User;
//using Elagy.Core.Entities;
//using Elagy.Core.Enums;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Elagy.BL.Mapping
//{
//    public class CarRentalProfile:Profile
//    {
//        public CarRentalProfile()
//        {

//            CreateMap<Driver, DriverResponseDto>()
//               .IncludeBase<User, BaseProfileDto>()
//               .ForMember(dest => dest.CarRentalAssetId, opt => opt.MapFrom(src => src.CarRentalAssetId))
//               .ForMember(dest => dest.CarRentalAssetName, opt => opt.MapFrom(src => src.CarRentalAsset.Name))
//               .ForMember(dest => dest.CarDrivers, opt => opt.MapFrom(src => src.CarDrivers));

//            // --- Car-Specific Mappings ---

//            // CarImage Entity to CarImageDto
//            CreateMap<CarImage, CarImageDto>();

//            CreateMap<CarCreateDto, Car>()
//               .ForMember(dest => dest.Id, opt => opt.Ignore())
//               .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => true))
//               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CarStatus.Available))
//               .ForMember(dest => dest.CarImages, opt => opt.Ignore())
//               .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore())
//               .ForMember(dest => dest.CarDrivers, opt => opt.Ignore());




//            // CarUpdateDto to Car Entity
//            CreateMap<CarUpdateDto, Car>()
//                    .ForMember(dest => dest.Id, opt => opt.Ignore())
//                    .ForMember(dest => dest.CarImages, opt => opt.Ignore())
//                    .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore())
//                    .ForMember(dest => dest.CarDrivers, opt => opt.Ignore());

//            CreateMap<Car, CarResponseDto>()
//            .ForMember(dest => dest.CarRentalAssetName, opt => opt.MapFrom(src => src.CarRentalAsset.Name))
//            .ForMember(dest => dest.CarImages, opt => opt.MapFrom(src => src.CarImages));

//            // --- CarDriver-Specific Mappings ---
//            CreateMap<CarDriverCreateDto, CarDriver>()
//                .ForMember(dest => dest.Id, opt => opt.Ignore())
//                .ForMember(dest => dest.AssignmentDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.Now)))
//                .ForMember(dest => dest.ReleaseDate, opt => opt.Ignore())
//                .ForMember(dest => dest.IsAssignedCurrent, opt => opt.MapFrom(src => true))
//                .ForMember(dest => dest.Car, opt => opt.Ignore())
//                .ForMember(dest => dest.Driver, opt => opt.Ignore())
//                .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore());

//            CreateMap<CarDriver, CarDriverResponseDto>()
//                 .ForMember(dest => dest.CarMake, opt => opt.MapFrom(src => src.Car.FactoryMake))
//                 .ForMember(dest => dest.CarModelName, opt => opt.MapFrom(src => src.Car.ModelName))
//                 .ForMember(dest => dest.DriverFirstName, opt => opt.MapFrom(src => src.Driver.FirstName))
//                 .ForMember(dest => dest.DriverLastName, opt => opt.MapFrom(src => src.Driver.LastName))
//                 .ForMember(dest => dest.CarRentalAssetId, opt => opt.MapFrom(src => src.CarRentalAssetId))
//                 .ForMember(dest => dest.CarRentalAssetName, opt => opt.MapFrom(src => src.CarRentalAsset.Name));

//            CreateMap<DriverCreateDto, Driver>()
//               .IncludeBase<User, BaseProfileDto>()
//               .ForMember(dest => dest.Id, opt => opt.Ignore())
//               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
//               .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
//               .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
//               .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
//               .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false))
//               .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => false))
//               .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => false))
//               .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => false))
//               .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => 0))
//               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Status.Active))
//               .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.Driver))
//               .ForMember(dest => dest.ImageId, opt => opt.Ignore())
//               .ForMember(dest => dest.ImageURL, opt => opt.Ignore())
//               .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore())
//               .ForMember(dest => dest.CarDrivers, opt => opt.Ignore())
//               .ForMember(dest => dest.Governorate, opt => opt.Ignore());



//            CreateMap<DriverUpdateDto, Driver>()
//                            .IncludeBase<User, BaseProfileDto>()
//                            .ForMember(dest => dest.Id, opt => opt.Ignore())
//                            .ForMember(dest => dest.Email, opt => opt.Ignore())
//                            .ForMember(dest => dest.UserName, opt => opt.Ignore())
//                            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
//                            .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
//                            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
//                            .ForMember(dest => dest.EmailConfirmed, opt => opt.Ignore())
//                            .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.Ignore())
//                            .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
//                            .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
//                            .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
//                            .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
//                            .ForMember(dest => dest.UserType, opt => opt.Ignore())
//                            .ForMember(dest => dest.ImageId, opt => opt.Ignore())
//                            .ForMember(dest => dest.ImageURL, opt => opt.Ignore())
//                            .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore())
//                            .ForMember(dest => dest.CarDrivers, opt => opt.Ignore())
//                            .ForMember(dest => dest.Governorate, opt => opt.Ignore());

//            // CarDriverCreateDto to CarDriver Entity
//            CreateMap<CarDriverCreateDto, CarDriver>()
//                .ForMember(dest => dest.Id, opt => opt.Ignore())
//                .ForMember(dest => dest.AssignmentDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.Now)))
//                .ForMember(dest => dest.ReleaseDate, opt => opt.Ignore())
//                .ForMember(dest => dest.IsAssignedCurrent, opt => opt.MapFrom(src => true))
//                .ForMember(dest => dest.Car, opt => opt.Ignore())
//                .ForMember(dest => dest.Driver, opt => opt.Ignore())
//                .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore());


//            // CarDriver Entity to CarDriverResponseDto
//            CreateMap<CarDriver, CarDriverResponseDto>()
//                .ForMember(dest => dest.CarMake, opt => opt.MapFrom(src => src.Car.FactoryMake))
//                .ForMember(dest => dest.CarModelName, opt => opt.MapFrom(src => src.Car.ModelName))
//                .ForMember(dest => dest.DriverFirstName, opt => opt.MapFrom(src => src.Driver.FirstName))
//                .ForMember(dest => dest.DriverLastName, opt => opt.MapFrom(src => src.Driver.LastName))
//                .ForMember(dest => dest.CarRentalAssetId, opt => opt.MapFrom(src => src.CarRentalAssetId))
//                .ForMember(dest => dest.CarRentalAssetName, opt => opt.MapFrom(src => src.CarRentalAsset.Name));
//        }

//    }
//}
