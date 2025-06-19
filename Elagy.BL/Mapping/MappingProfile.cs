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
            // --- Entity to DTO Mappings (Profile DTOs) ---

            // Base User to BaseProfileDto
            CreateMap<User, BaseProfileDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phone)) // Map Phone to PhoneNumber
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageURL))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType))
                .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => src.Governorate))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserName)) // IdentityUser.UserName is the Email
                //.ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
                .IncludeAllDerived(); // Important for TPH inheritance

            // Patient to PatientDto
            CreateMap<Patient, PatientDto>();

            // SuperAdmin to SuperAdminDto
            CreateMap<SuperAdmin, SuperAdminDto>();

            // Asset to BaseServiceProviderProfileDto (flattening asset details) - used for generic asset mapping
            CreateMap<Asset, BaseServiceProviderProfileDto>()
                .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CredentialDocURL, opt => opt.MapFrom(src => src.CredentialDocURL))
                .ForMember(dest => dest.AssetEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.AcquisitionDate, opt => opt.MapFrom(src => src.AcquisitionDate))
                .ForMember(dest => dest.VerificationNotes, opt => opt.MapFrom(src => src.VerificationNotes))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForMember(dest => dest.LocationDescription, opt => opt.MapFrom(src => src.LocationDescription))
                .ForMember(dest => dest.Facilities, opt => opt.MapFrom(src => src.Facilities))
                .ForMember(dest => dest.OpeningTime, opt => opt.MapFrom(src => src.OpeningTime))
                .ForMember(dest => dest.ClosingTime, opt => opt.MapFrom(src => src.ClosingTime))
                .ForMember(dest => dest.LanguagesSupported, opt => opt.MapFrom(src => src.LanguagesSupported))
                .ForMember(dest => dest.AssetType, opt => opt.MapFrom(src => src.AssetType))
                .ReverseMap();

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
                .ForMember(dest => dest.AcquisitionDate, opt => opt.MapFrom(src => src.ServiceAsset.AcquisitionDate))
                .ForMember(dest => dest.VerificationNotes, opt => opt.MapFrom(src => src.ServiceAsset.VerificationNotes))
                .ForMember(dest => dest.AssetType, opt => opt.MapFrom(src => src.ServiceAsset.AssetType))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.ServiceAsset.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.ServiceAsset.Longitude))
                .ForMember(dest => dest.LocationDescription, opt => opt.MapFrom(src => src.ServiceAsset.LocationDescription))
                .ForMember(dest => dest.Facilities, opt => opt.MapFrom(src => src.ServiceAsset.Facilities))
                .ForMember(dest => dest.OpeningTime, opt => opt.MapFrom(src => src.ServiceAsset.OpeningTime))
                .ForMember(dest => dest.ClosingTime, opt => opt.MapFrom(src => src.ServiceAsset.ClosingTime))
                .ForMember(dest => dest.LanguagesSupported, opt => opt.MapFrom(src => src.ServiceAsset.LanguagesSupported));

            // Specific ServiceProvider types to their combined Profile DTOs
            CreateMap<Elagy.Core.Entities.ServiceProvider, HotelProviderProfileDto>()
                .IncludeBase<Elagy.Core.Entities.ServiceProvider, BaseServiceProviderProfileDto>()
                .ForMember(dest => dest.StarRating, opt => opt.MapFrom(src => (src.ServiceAsset as HotelAsset).StarRating))
                .ForMember(dest => dest.HasPool, opt => opt.MapFrom(src => (src.ServiceAsset as HotelAsset).HasPool))
                .ForMember(dest => dest.HasRestaurant, opt => opt.MapFrom(src => (src.ServiceAsset as HotelAsset).HasRestaurant));

            CreateMap<Elagy.Core.Entities.ServiceProvider, HospitalProviderProfileDto>()
                .IncludeBase<Elagy.Core.Entities.ServiceProvider, BaseServiceProviderProfileDto>()
                .ForMember(dest => dest.NumberOfDepartments, opt => opt.MapFrom(src => (src.ServiceAsset as HospitalAsset).NumberOfDepartments))
                .ForMember(dest => dest.EmergencyServices, opt => opt.MapFrom(src => (src.ServiceAsset as HospitalAsset).EmergencyServices));

            CreateMap<Elagy.Core.Entities.ServiceProvider, CarRentalProviderProfileDto>()
                .IncludeBase<Elagy.Core.Entities.ServiceProvider, BaseServiceProviderProfileDto>()
                .ForMember(dest => dest.OperationalAreas, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).OperationalAreas))
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).Models))
                .ForMember(dest => dest.Transmission, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).Transmission))
                .ForMember(dest => dest.FuelTypes, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).FuelTypes))
                .ForMember(dest => dest.RentalPolicies, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).RentalPolicies));


            // --- DTO to Entity Mappings (for Registration and Updates) ---

            // Base Registration Request DTOs to Entities
            CreateMap<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) // UserName is used for login, typically the email
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false)) // Initial state
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore()) // Managed by Identity
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore()) // Managed by Identity
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => true)) // Enable lockout by default
                .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => false)) // Initial state
                                                                                                // Address, City, Governorate, Country, DateOfBirth, FirstName, LastName, Gender map by convention
                .IncludeAllDerived();

            CreateMap<PatientRegistrationRequestDto, Patient>()
                .IncludeBase<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.Patient))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserStatus.EmailUnconfirmed)); // Initial status

            CreateMap<SuperAdminRegistrationRequestDto, SuperAdmin>()
                .IncludeBase<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.SuperAdmin))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserStatus.EmailUnconfirmed)); // Email still needs confirmation

            // Base Asset Registration Request DTO to ServiceProvider (User part)
            CreateMap<BaseAssetRegistrationRequestDto, Elagy.Core.Entities.ServiceProvider>()
                .IncludeBase<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.ServiceProvider))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserStatus.EmailUnconfirmed)); // Initial status

            // Base Asset Registration Request DTO to Asset (Asset part)
            CreateMap<BaseAssetRegistrationRequestDto, Asset>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AssetEmail)) 
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AssetName))  
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.AssetDescription))  
                .ForMember(dest => dest.AcquisitionDate, opt => opt.MapFrom(src => DateTime.UtcNow)) // Set acquisition date on creation
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID will be set to ServiceProvider's ID
                .ForMember(dest => dest.ServiceProvider, opt => opt.Ignore()) // Will be set manually
                                                                              // AssetName, AssetDescription, LocationDescription, Latitude, Longitude, Facilities, OpeningTime, ClosingTime, LanguagesSupported map by convention
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
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                // Explicitly map properties that are present in the DTO but not directly inherited or handled by Identity
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.ImageId, opt => opt.MapFrom(src => src.ImageId))
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageURL))
                // Note: PatientProfileUpdateDto has 'StreetNumber' but Patient entity has 'Address'.
                // Assuming 'StreetNumber' from DTO maps to 'Address' in Entity, adjust if this is not the case.
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.StreetNumber))
                // DTO has Governorate as string, Entity has Enum. Convert.
                .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => Enum.Parse<Governorate>(src.Governorate)))
                // City and Country are in BaseProfileDto, but not in this specific update DTO.
                // If they are meant to be updatable, they should be added to PatientProfileUpdateDto.
                .ForMember(dest => dest.City, opt => opt.Ignore())
                .ForMember(dest => dest.Country, opt => opt.Ignore());
            // BloodGroup, Height, Weight can be direct mapped as names match

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
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                // Map common user properties for update
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => src.Governorate))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth));

            // Mapping from BaseServiceProviderProfileUpdateDto to Asset (for common asset updates)
            CreateMap<BaseServiceProviderProfileUpdateDto, Asset>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID remains same
                 .ForMember(dest => dest.AcquisitionDate, opt => opt.Ignore()) // Date fixed on creation
                 .ForMember(dest => dest.AssetType, opt => opt.Ignore()) // Type fixed on creation
                 .ForMember(dest => dest.VerificationNotes, opt => opt.Ignore()) // VerificationNotes typically updated separately/internally
                 .ForMember(dest => dest.CredentialDocURL, opt => opt.Ignore()) // Credential docs updated separately
                 .ForMember(dest => dest.CredentialDocId, opt => opt.Ignore()) // Credential docs updated separately
                 .ForMember(dest => dest.Email, opt => opt.Ignore()) // Asset-specific email if it can be updated
                 .ForMember(dest => dest.ServiceProvider, opt => opt.Ignore()) // Navigation property ignored
                 .ForMember(dest => dest.Name, opt => opt.Ignore()) // Assuming Name and Description are not updated via this DTO
                 .ForMember(dest => dest.Description, opt => opt.Ignore())
                 // Direct mappings for common asset update properties
                 .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                 .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                 .ForMember(dest => dest.LocationDescription, opt => opt.MapFrom(src => src.LocationDescription))
                 .ForMember(dest => dest.Facilities, opt => opt.MapFrom(src => src.Facilities))
                 .ForMember(dest => dest.LanguagesSupported, opt => opt.MapFrom(src => src.LanguagesSupported))
                 .ForMember(dest => dest.OpeningTime, opt => opt.MapFrom(src => src.OpeningTime))
                 .ForMember(dest => dest.ClosingTime, opt => opt.MapFrom(src => src.ClosingTime));


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