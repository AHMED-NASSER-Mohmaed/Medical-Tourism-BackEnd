using AutoMapper;
using Elagy.Core.Entities;
using Elagy.Core.DTOs.Auth; // Now we have all these DTOs explicitly
using Elagy.Core.DTOs.User;
using Elagy.Core.Enums;
using System; // For DateTime and TimeOnly

namespace Elagy.BL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {


            #region--- Entity to DTO Mappings (Profile DTOs) ---

            // Base User to BaseProfileDto
            CreateMap<User, BaseProfileDto>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(src => src.Governorate.CountryId))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Governorate.Country.Name))
                .ForMember(dest => dest.GovernorateId, opt => opt.MapFrom(src => src.GovernorateId))
                .ForMember(dest => dest.GovernorateName, opt => opt.MapFrom(src => src.Governorate.Name))


                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserName)) // IdentityUser.UserName is the Email
                .IncludeAllDerived(); // Important for TPH inheritance

            // Patient to PatientDto
            CreateMap<Patient, PatientDto>();

            // SuperAdmin to SuperAdminDto
            CreateMap<SuperAdmin, SuperAdminDto>();



            // ServiceProvider to BaseServiceProviderProfileDto
            // This mapping flattens the ServiceAsset properties into the DTO.
            CreateMap<Elagy.Core.Entities.ServiceProvider, BaseServiceProviderProfileDto>()
                .IncludeBase<User, BaseProfileDto>()
                .ForMember(dest => dest.NationalDocsURL, opt => opt.MapFrom(src => src.NationalURL))
                // Map Asset properties from ServiceProvider.ServiceAsset
                .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.ServiceAsset.Id))
                .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.ServiceAsset.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ServiceAsset.Description))
                .ForMember(dest => dest.CredentialDocURL, opt => opt.MapFrom(src => src.ServiceAsset.CredentialDocURL))
                .ForMember(dest => dest.AssetEmail, opt => opt.MapFrom(src => src.ServiceAsset.Email))
                .ForMember(dest => dest.AssetGovernateId, opt => opt.MapFrom(src => src.ServiceAsset.GovernateId))
                .ForMember(dest => dest.AssetGovernateName, opt => opt.MapFrom(src => src.ServiceAsset.Governate.Name));





            // Specific ServiceProvider types to their combined Profile DTOs
            CreateMap<Elagy.Core.Entities.ServiceProvider, HotelProviderProfileDto>()
                .IncludeBase<Elagy.Core.Entities.ServiceProvider, BaseServiceProviderProfileDto>()
                .ForMember(dest => dest.StarRating, opt => opt.MapFrom(src => (src.ServiceAsset as HotelAsset).StarRating))
                .ForMember(dest => dest.HasPool, opt => opt.MapFrom(src => (src.ServiceAsset as HotelAsset).HasPool))
                .ForMember(dest => dest.HasRestaurant, opt => opt.MapFrom(src => (src.ServiceAsset as HotelAsset).HasRestaurant))
                   .ForMember(dest => dest.AssetImages, opt => opt.MapFrom(src =>
                    (src.ServiceAsset as HotelAsset).HotelAssetImages));

            CreateMap<Elagy.Core.Entities.ServiceProvider, HospitalProviderProfileDto>()
                .IncludeBase<Elagy.Core.Entities.ServiceProvider, BaseServiceProviderProfileDto>()
                .ForMember(dest => dest.NumberOfDepartments, opt => opt.MapFrom(src => (src.ServiceAsset as HospitalAsset).NumberOfDepartments))
                .ForMember(dest => dest.EmergencyServices, opt => opt.MapFrom(src => (src.ServiceAsset as HospitalAsset).EmergencyServices))
                  .ForMember(dest => dest.AssetImages, opt => opt.MapFrom(src =>
                    (src.ServiceAsset as HospitalAsset).HospitalAssetImages));

            CreateMap<Elagy.Core.Entities.ServiceProvider, CarRentalProviderProfileDto>()
                .IncludeBase<Elagy.Core.Entities.ServiceProvider, BaseServiceProviderProfileDto>()
                //.ForMember(dest => dest.OperationalAreas, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).OperationalAreas))
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).Models))
                .ForMember(dest => dest.Transmission, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).Transmission))
                .ForMember(dest => dest.FuelTypes, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).FuelTypes))
                .ForMember(dest => dest.RentalPolicies, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).RentalPolicies)).
                   ForMember(dest => dest.AssetImages, opt => opt.MapFrom(src =>
                    (src.ServiceAsset as CarRentalAsset).CarRentalAssetImages));

            #endregion







            #region --- DTO to Entity Mappings (for Registration and Updates) ---

            // Base Registration Request DTOs to Entities
            CreateMap<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) // UserName is used for login, typically the email
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false)) // Initial state
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore()) // Managed by Identity
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore()) // Managed by Identity
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => true)) // Enable lockout by default
                .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => false)) // Initial state
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Status.EmailUnconfirmed))
                .IncludeAllDerived();



            CreateMap<PatientRegistrationRequestDto, Patient>()
                .IncludeBase<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.Patient));




            CreateMap<SuperAdminRegistrationRequestDto, SuperAdmin>()
                .IncludeBase<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.SuperAdmin));



            // Base Asset Registration Request DTO to ServiceProvider (User part)
            CreateMap<BaseAssetRegistrationRequestDto, Elagy.Core.Entities.ServiceProvider>()
                .IncludeBase<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.ServiceProvider));

            #endregion






            #region asset mapping 
            /*****************************************************************************/


            // Base Asset Registration Request DTO to Asset (Asset part)
            CreateMap<BaseAssetRegistrationRequestDto, Asset>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AssetEmail)) 
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AssetName))  
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.AssetDescription))  
                .ForMember(dest => dest.GovernateId, opt => opt.MapFrom(src => src.AssetGovernorateId))  
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID will be set to ServiceProvider's ID
                .ForMember(dest => dest.ServiceProvider, opt => opt.Ignore()) // Will be set manually
                .IncludeAllDerived();

            CreateMap<HotelAssetRegistrationRequestDto, HotelAsset>()
                .IncludeBase<BaseAssetRegistrationRequestDto, Asset>()
                .ForMember(dest => dest.AssetType, opt => opt.MapFrom(src => AssetType.Hotel));



            CreateMap<HospitalAssetRegistrationRequestDto, HospitalAsset>()
                .IncludeBase<BaseAssetRegistrationRequestDto, Asset>()
                .ForMember(dest => dest.AssetType, opt => opt.MapFrom(src => AssetType.Hospital));



            CreateMap<CarRentalAssetRegistrationRequestDto, CarRentalAsset>()
                .IncludeBase<BaseAssetRegistrationRequestDto, Asset>()
                .ForMember(dest => dest.AssetType, opt => opt.MapFrom(src => AssetType.CarRental));



            /*****************************************************************************/
            #endregion




            // Update DTOs to Entities (for Profile Updates)

            // PatientProfileUpdateDto to Patient
            CreateMap<PatientProfileUpdateDto, Patient>()
                .ForMember(dest => dest.Email, opt => opt.Ignore()) // Email is changed via ChangeEmailAsync
                .ForMember(dest => dest.UserName, opt => opt.Ignore()) // Username (email) ignored
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Password ignored
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore());








            // BaseServiceProviderProfileUpdateDto to ServiceProvider & ServiceAsset (for flattened update)
            CreateMap<BaseServiceProviderProfileUpdateDto, Elagy.Core.Entities.ServiceProvider>()
                // Ignore Identity properties as they are not updated via profile DTOs
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore());



            // Mapping from BaseServiceProviderProfileUpdateDto to Asset (for common asset updates)
            CreateMap<BaseServiceProviderProfileUpdateDto, Asset>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore())  
                 .ForMember(dest => dest.AssetType, opt => opt.Ignore())  
                 .ForMember(dest => dest.Email, opt => opt.Ignore())  
                 .ForMember(dest => dest.ServiceProvider, opt => opt.Ignore())  
                 .ForMember(dest => dest.Description, opt => opt.Ignore())
                 // Direct mappings for common asset update properties
                 .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AssetName))
                 .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.AsetDescription));
                    


            // Specific Provider Update DTOs to specific Asset Entities
            CreateMap<HotelProviderProfileUpdateDto, HotelAsset>()
                .IncludeBase<BaseServiceProviderProfileUpdateDto, Asset>(); // Inherits common asset update properties

            CreateMap<HospitalProviderProfileUpdateDto, HospitalAsset>()
                .IncludeBase<BaseServiceProviderProfileUpdateDto, Asset>();

            CreateMap<CarRentalProviderProfileUpdateDto, CarRentalAsset>()
                .IncludeBase<BaseServiceProviderProfileUpdateDto, Asset>();



        }
    }
}