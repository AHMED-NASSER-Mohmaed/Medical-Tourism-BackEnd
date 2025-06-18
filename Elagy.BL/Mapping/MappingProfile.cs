using AutoMapper;
using Elagy.Core.Entities;
using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.User;
using Elagy.Core.Enums; // For UserType, AssetType
using ServiceProvider = Elagy.Core.Entities.ServiceProvider; // Ensure this is the correct namespace for ServiceProvider
//due to the confustion that happent between the serviceprovider  injection service 
namespace Elagy.BL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- Entity to DTO Mappings ---

            // Base User to BaseProfileDto
            CreateMap<User, BaseProfileDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName)) 
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status)) 
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))  
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageURL))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType))
                .ForMember(dest => dest.Governorate, opt => opt.MapFrom(src => src.Governorate))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country)) 
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))  
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))   
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserName)) 
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.EmailConfirmed))
                .IncludeAllDerived(); // Important for TPH inheritance

            // Patient to PatientDto
            CreateMap<Patient, PatientDto>();

            // SuperAdmin to SuperAdminDto
            CreateMap<SuperAdmin, SuperAdminDto>();

            // ServiceAsset to BaseServiceProviderProfileDto (flattening asset details)
            CreateMap<Asset, BaseServiceProviderProfileDto>()
                .ForMember(dest => dest.AssetId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AssetEmail, opt => opt.MapFrom(src => src.Email)) // Map asset-specific email
                .ForMember(dest => dest.CredentialDocURL, opt => opt.MapFrom(src => src.CredentialDocURL))
                .ReverseMap(); // Allows mapping back from DTO to Entity

            // ServiceProvider to BaseServiceProviderProfileDto
            CreateMap<ServiceProvider, BaseServiceProviderProfileDto>()
                .IncludeBase<User, BaseProfileDto>() // Include mappings from BaseProfileDto
                .ForMember(dest => dest.NationalDocsURL, opt => opt.MapFrom(src => src.NationalURL))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ServiceAsset.Id))
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
            CreateMap<ServiceProvider, HotelProviderProfileDto>();
            CreateMap<ServiceProvider, HospitalProviderProfileDto>();
            CreateMap<ServiceProvider, CarRentalProviderProfileDto>();

            // Important: Handle specific asset properties when mapping ServiceProvider to derived ProviderProfileDto
            // This needs specific handling because AutoMapper won't automatically 'flatten' nested specific asset props.
            // When mapping from ServiceProvider to a *specific* provider profile DTO, you need to map the asset's specific properties
            CreateMap<ServiceProvider, HotelProviderProfileDto>()
                .ForMember(dest => dest.StarRating, opt => opt.MapFrom(src => (src.ServiceAsset as HotelAsset).StarRating))
                .ForMember(dest => dest.HasPool, opt => opt.MapFrom(src => (src.ServiceAsset as HotelAsset).HasPool))
                .ForMember(dest => dest.HasRestaurant, opt => opt.MapFrom(src => (src.ServiceAsset as HotelAsset).HasRestaurant))
                .IncludeBase<ServiceProvider, BaseServiceProviderProfileDto>(); // Inherit common SP/Asset properties

            CreateMap<ServiceProvider, HospitalProviderProfileDto>()
                .ForMember(dest => dest.NumberOfDepartments, opt => opt.MapFrom(src => (src.ServiceAsset as HospitalAsset).NumberOfDepartments))
                .ForMember(dest => dest.EmergencyServices, opt => opt.MapFrom(src => (src.ServiceAsset as HospitalAsset).EmergencyServices))
                .IncludeBase<ServiceProvider, BaseServiceProviderProfileDto>();

            CreateMap<ServiceProvider, CarRentalProviderProfileDto>()
                .ForMember(dest => dest.OperationalAreas, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).OperationalAreas))
                .ForMember(dest => dest.Models, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).Models))
                .ForMember(dest => dest.Transmission, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).Transmission))
                .ForMember(dest => dest.FuelTypes, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).FuelTypes))
                .ForMember(dest => dest.RentalPolicies, opt => opt.MapFrom(src => (src.ServiceAsset as CarRentalAsset).RentalPolicies))
                .IncludeBase<ServiceProvider, BaseServiceProviderProfileDto>();











            // --- DTO to Entity Mappings (for Registration and Updates) ---

            // Registration Request DTOs to Entities
            CreateMap<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)) // late iwill remove it 
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) // Often username is email
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => false)) // Initially false
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore()) // Identity handles this
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore()) // Identity handles this
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.MapFrom(src => true)) // Enable lockout by default
                .ForMember(dest => dest.AccessFailedCount, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => false))
                .IncludeAllDerived(); // For registration

            CreateMap<PatientRegistrationRequestDto, Patient>()
                .IncludeBase<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.Patient))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserStatus.EmailUnconfirmed)); // Initial status

            CreateMap<BaseAssetRegistrationRequestDto, ServiceProvider>()
                .IncludeBase<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.ServiceProvider))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserStatus.EmailUnconfirmed)); // Initial status

            // Map registration DTOs to specific ServiceAsset types
            CreateMap<BaseAssetRegistrationRequestDto, Asset>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AssetEmail)) // Map asset-specific email
                .ForMember(dest => dest.AcquisitionDate, opt => opt.MapFrom(src => DateTime.UtcNow)) // Set acquisition date on creation
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID will be set to ServiceProvider's ID
                .ForMember(dest => dest.ServiceProvider, opt => opt.Ignore()) // Will be set manually
                .IncludeAllDerived();

            CreateMap<HotelAssetRegistrationRequestDto, HotelAsset>()
                .IncludeBase<BaseAssetRegistrationRequestDto, Asset>()
                .ForMember(dest => dest.AssetType, opt => opt.MapFrom(src => AssetType.Hotel));

            CreateMap<HospitalAssetRegistrationRequestDto, HospitalAsset>()
                .IncludeBase<BaseAssetRegistrationRequestDto, Asset>()
                .ForMember(dest => dest.AssetType, opt => opt.MapFrom(src => AssetType.Hospital)); // why i put it here because the basse type dose not know any thing about his child 

            CreateMap<CarRentalAssetRegistrationRequestDto, CarRentalAsset>()
                .IncludeBase<BaseAssetRegistrationRequestDto, Asset>()
                .ForMember(dest => dest.AssetType, opt => opt.MapFrom(src => AssetType.CarRental));

            CreateMap<SuperAdminRegistrationRequestDto, SuperAdmin>()
                .IncludeBase<BaseRegistrationRequestDto, User>()
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.SuperAdmin))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserStatus.EmailUnconfirmed)); // Email still needs confirmation


            // Update DTOs to Entities (for Profile Updates)
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
            CreateMap<BaseServiceProviderProfileUpdateDto, ServiceProvider>()
                .ForMember(dest => dest.Email, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFactorEnabled, opt => opt.Ignore());


            CreateMap<BaseServiceProviderProfileUpdateDto, Asset>()
                 .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID remains same
                 .ForMember(dest => dest.AcquisitionDate, opt => opt.Ignore()) // Date fixed on creation
                 .ForMember(dest => dest.AssetType, opt => opt.Ignore()); // Type fixed on creation

            // Specific Provider Update DTOs to specific Asset Entities
            CreateMap<HotelProviderProfileUpdateDto, HotelAsset>()
                .IncludeBase<BaseServiceProviderProfileUpdateDto, Asset>();

            CreateMap<HospitalProviderProfileUpdateDto, HospitalAsset>()
                .IncludeBase<BaseServiceProviderProfileUpdateDto, Asset>();

            CreateMap<CarRentalProviderProfileUpdateDto, CarRentalAsset>()
                .IncludeBase<BaseServiceProviderProfileUpdateDto, Asset>();
        }
    }
}