using AutoMapper;
using Elagy.Core.DTOs.CarRentals;
using Elagy.Core.DTOs.Driver;
using Elagy.Core.DTOs.Room;
using Elagy.Core.DTOs.User;
using Elagy.Core.Entities;
using Elagy.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace Elagy.BL.Mapping
{
    public class CarRentalProfile : Profile
    {
        public CarRentalProfile()
        {

            CreateMap<Driver, DriverResponseDto>()
               .ForMember(dest => dest.CarRentalAssetId, opt => opt.MapFrom(src => src.CarRentalAssetId))
               .ForMember(dest => dest.CarRentalAssetName, opt => opt.MapFrom(src => src.CarRentalAsset.Name))
               .ForMember(dest => dest.CarDrivers, opt => opt.MapFrom(src => src.CarDrivers))
                  .ForMember(dest => dest.governerateId, opt => opt.MapFrom(src => src.GovernorateId))
    .ForMember(dest => dest.countryId, opt => opt.MapFrom(src => src.Governorate.CountryId))
    .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Governorate.Country.Name))
    .ForMember(dest => dest.GovernarteName, opt => opt.MapFrom(src => src.Governorate.Name));



            // --- Car-Specific Mappings ---

            // CarImage Entity to CarImageDto
            CreateMap<CarImage, CarImageDto>();

            CreateMap<CarCreateDto, Car>()
               .ForMember(dest => dest.Id, opt => opt.Ignore())


               .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => true))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CarStatus.Available))
               .ForMember(dest => dest.CarImages, opt => opt.Ignore())
               .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore())
               .ForMember(dest => dest.CarDrivers, opt => opt.Ignore());




            // CarUpdateDto to Car Entity
            CreateMap<CarUpdateDto, Car>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())


                    .ForMember(dest => dest.CarImages, opt => opt.Ignore())
                    .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore())
                    .ForMember(dest => dest.CarDrivers, opt => opt.Ignore());

            CreateMap<Car, CarResponseDto>()
            .ForMember(dest => dest.CarRentalAssetName, opt => opt.MapFrom(src => src.CarRentalAsset.Name))


            .ForMember(dest => dest.CarImages, opt => opt.MapFrom(src => src.CarImages));

            // --- CarDriver-Specific Mappings ---
            CreateMap<CarDriverCreateDto, CarDriver>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AssignmentDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.Now)))
                .ForMember(dest => dest.ReleaseDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsAssignedCurrent, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Car, opt => opt.Ignore())
                .ForMember(dest => dest.Driver, opt => opt.Ignore())
                .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore());

            CreateMap<CarDriver, CarDriverResponseDto>()
                 .ForMember(dest => dest.CarMake, opt => opt.MapFrom(src => src.Car.FactoryMake))
                 .ForMember(dest => dest.CarModelName, opt => opt.MapFrom(src => src.Car.ModelName))
                 .ForMember(dest => dest.DriverFirstName, opt => opt.MapFrom(src => src.Driver.FirstName))
                 .ForMember(dest => dest.DriverLastName, opt => opt.MapFrom(src => src.Driver.LastName))
                 .ForMember(dest => dest.CarRentalAssetId, opt => opt.MapFrom(src => src.CarRentalAssetId))
                 .ForMember(dest => dest.CarRentalAssetName, opt => opt.MapFrom(src => src.CarRentalAsset.Name));

            CreateMap<DriverCreateDto, Driver>()

               .ForMember(dest => dest.Id, opt => opt.Ignore())
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
               .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
               .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
               .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true))
               .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => true))
               .ForMember(dest => dest.TwoFactorEnabled, opt => opt.MapFrom(src => true))
               .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => true))
               .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => 0))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Status.Active))
               .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.Driver))
               .ForMember(dest => dest.ImageId, opt => opt.Ignore())
               .ForMember(dest => dest.ImageURL, opt => opt.Ignore())
               .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore())
               .ForMember(dest => dest.CarDrivers, opt => opt.Ignore())
                 .ForMember(dest => dest.Governorate, opt => opt.Ignore())
               .ForMember(dest => dest.GovernorateId, opt => opt.MapFrom(src => src.UserGovernorateId));






            //public DateTime?  { get; set; }
            //public Status? Status { get; set; } // User's general account status
            //[Range(0, 70)] public int? YearsOfExperience { get; set; }
            //[Range(0.0f, 5.0f)] public float? Rating { get; set; }
            //public DriverStatus? DriverStatus { get; set; }


            CreateMap<DriverUpdateDto, Driver>()

                                .ForMember(dest => dest.Id, opt => opt.Ignore())
                                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName)).
                                 ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                                 .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                                 .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                                 .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                                 .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                                 .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                                 .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                                 .ForMember(dest => dest.YearsOfExperience, opt => opt.MapFrom(src => src.YearsOfExperience))
                                 .ForMember(dest => dest.DriverStatus, opt => opt.MapFrom(src => src.DriverStatus))
                                 .ForMember(dest => dest.GovernorateId, opt => opt.MapFrom(src => src.UserGovernorateId))
                                .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore())
                                .ForMember(dest => dest.CarDrivers, opt => opt.Ignore())
                                .ForMember(dest => dest.Governorate, opt => opt.Ignore());

            // CarDriverCreateDto to CarDriver Entity
            CreateMap<CarDriverCreateDto, CarDriver>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.AssignmentDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(DateTime.Now)))
                .ForMember(dest => dest.ReleaseDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsAssignedCurrent, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Car, opt => opt.Ignore())
                .ForMember(dest => dest.Driver, opt => opt.Ignore())
                .ForMember(dest => dest.CarRentalAsset, opt => opt.Ignore());


            // CarDriver Entity to CarDriverResponseDto
            CreateMap<CarDriver, CarDriverResponseDto>()
                .ForMember(dest => dest.CarMake, opt => opt.MapFrom(src => src.Car.FactoryMake))
                .ForMember(dest => dest.CarModelName, opt => opt.MapFrom(src => src.Car.ModelName))
                .ForMember(dest => dest.DriverFirstName, opt => opt.MapFrom(src => src.Driver.FirstName))
                .ForMember(dest => dest.DriverLastName, opt => opt.MapFrom(src => src.Driver.LastName))
                .ForMember(dest => dest.CarRentalAssetId, opt => opt.MapFrom(src => src.CarRentalAssetId))
                .ForMember(dest => dest.CarRentalAssetName, opt => opt.MapFrom(src => src.CarRentalAsset.Name));
        }

    }
}
