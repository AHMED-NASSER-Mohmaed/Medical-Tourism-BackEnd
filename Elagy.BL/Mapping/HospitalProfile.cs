using AutoMapper;
using Elagy.Core.DTOs.Auth;
using Elagy.Core.DTOs.Doctor;
using Elagy.Core.DTOs.DoctorSchedule;
using Elagy.Core.DTOs.Specialty;
using Elagy.Core.DTOs.User;
using Elagy.Core.Entities;
using Elagy.Core.Enums; // For UserType, AssetType
using ServiceProvider = Elagy.Core.Entities.ServiceProvider; // Ensure this is the correct namespace for ServiceProvider
//due to the confustion that happent between the serviceprovider  injection service 
namespace Elagy.BL.Mapping
{
    public class HospitalProfile:Profile
    {
        public HospitalProfile()
        {
         
            CreateMap<Specialty, SpecialtyDto>().ReverseMap(); // For CRUD and nested views
            CreateMap<SpecialtyCreateDto, Specialty>();
            CreateMap<SpecialtyUpdateDto, Specialty>();
            //------------------------Doctor--------------------
            CreateMap<Doctor, DoctorDto>()
               .IncludeBase<User, Elagy.Core.DTOs.User.BaseProfileDto>() // Doctor is a User, maps base User properties to BaseProfileDto structure
                                                                         // Map properties specific to Doctor entity that are directly on DoctorDto
               .ForMember(dest => dest.MedicalLicenseNumber, opt => opt.MapFrom(src => src.MedicalLicenseNumber))
               .ForMember(dest => dest.YearsOfExperience, opt => opt.MapFrom(src => src.YearsOfExperience))
               .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
               .ForMember(dest => dest.Qualification, opt => opt.MapFrom(src => src.Qualification))
               // Map properties from HospitalSpecialty navigation property
               .ForMember(dest => dest.SpecialtyId, opt => opt.MapFrom(src => src.HospitalSpecialty.SpecialtyId))
               .ForMember(dest => dest.SpecialtyName, opt => opt.MapFrom(src => src.HospitalSpecialty.Specialty.Name))
               .ForMember(dest => dest.HospitalAssetId, opt => opt.MapFrom(src => src.HospitalSpecialty.HospitalAssetId))
               .ForMember(dest => dest.HospitalName, opt => opt.MapFrom(src => (src.HospitalSpecialty.HospitalAsset as HospitalAsset).Name)); // Cast to HospitalAsset to get AssetName
                                                                                                                                                   // IMPORTANT: For this mapping to work, ensure HospitalSpecialty and Specialty/HospitalAsset are EAGERLY LOADED.

            CreateMap<Doctor, DoctorTableDto>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName)) // Concatenate name
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
              .ForMember(dest => dest.Specialist, opt => opt.MapFrom(src => src.HospitalSpecialty.Specialty.Name)); // Requires HospitalSpecialty and Specialty to be loaded

            CreateMap<DoctorCreateDto, Doctor>()
               .IncludeBase<BaseRegistrationRequestDto, User>() // Map base registration DTO properties to User entity
                                                                // Map properties specific to Doctor entity
               .ForMember(dest => dest.MedicalLicenseNumber, opt => opt.MapFrom(src => src.MedicalLicenseNumber))
               .ForMember(dest => dest.YearsOfExperience, opt => opt.MapFrom(src => src.YearsOfExperience))
               .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
               .ForMember(dest => dest.Qualification, opt => opt.MapFrom(src => src.Qualification))
               .ForMember(dest => dest.HospitalSpecialtyId, opt => opt.MapFrom(src => src.HospitalSpecialtyId)) // Direct FK map
               .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email)) // Set UserName from Email
               .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => UserType.Doctor)) // Set specific UserType
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => UserStatus.Active)) // Default status on creation
               .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true)) // Confirmed by admin
               .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Password set by UserManager


            CreateMap<DoctorUpdateDto, Doctor>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Map the ID for finding the doctor
                                                                              // Map properties inherited from User (explicitly for update, or AutoMapper can infer matching names)
               .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
               .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email)) // Email might be updated
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
               .ForMember(dest => dest.ImageURL, opt => opt.MapFrom(src => src.ImageURL))
               .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
               .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
               // Address/City/Gov/Country are not in DoctorUpdateDto
               .ForMember(dest => dest.Address, opt => opt.Ignore()) // Ignore if not updating through this DTO
               .ForMember(dest => dest.City, opt => opt.Ignore())
               .ForMember(dest => dest.Governorate, opt => opt.Ignore())
               .ForMember(dest => dest.Country, opt => opt.Ignore())
               // Map properties specific to Doctor entity
               .ForMember(dest => dest.MedicalLicenseNumber, opt => opt.MapFrom(src => src.MedicalLicenseNumber))
               .ForMember(dest => dest.YearsOfExperience, opt => opt.MapFrom(src => src.YearsOfExperience))
               .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio))
               .ForMember(dest => dest.Qualification, opt => opt.MapFrom(src => src.Qualification))
               .ForMember(dest => dest.HospitalSpecialtyId, opt => opt.MapFrom(src => src.HospitalSpecialtyId)); // Update specialty FK

            // For mapping HospitalSpecialty entity to a DTO for dropdowns etc.
            CreateMap<HospitalSpecialty, HospitalSpecialtyDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.HospitalAssetId, opt => opt.MapFrom(src => src.HospitalAssetId))
                .ForMember(dest => dest.SpecialtyId, opt => opt.MapFrom(src => src.SpecialtyId))
                .ForMember(dest => dest.Specialty, opt => opt.MapFrom(src => src.Specialty)) // Map nested Specialty
                .ForMember(dest => dest.HospitalAsset, opt => opt.MapFrom(src => src.HospitalAsset)); // Map nested HospitalAsset

            // Helper DTOs for nested mappings (if they don't have their own profiles)
            CreateMap<Specialty, SpecialtyDto>();
            CreateMap<HospitalAsset, HospitalMinDto>(); // HospitalMinDto likely needs AssetName
            CreateMap<Asset, HospitalMinDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name)); // Assuming Name in DTO maps to AssetName in Entity

            CreateMap<DoctorCreateDto, DoctorTableDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Specialist, opt => opt.Ignore()); // Specialist name is not in DoctorCreateDto


            CreateMap<Schedule, ScheduleDto>()
            .ForMember(dest => dest.DoctorFirstName, opt => opt.MapFrom(src => src.Doctor.FirstName))
            .ForMember(dest => dest.DoctorLastName, opt => opt.MapFrom(src => src.Doctor.LastName))
            .ForMember(dest => dest.HospitalName, opt => opt.MapFrom(src => src.HospitalSpecialty.HospitalAsset.Name))
            .ForMember(dest => dest.SpecialtyName, opt => opt.MapFrom(src => src.HospitalSpecialty.Specialty.Name));



            CreateMap<Schedule, ScheduleDto>()
    .ForMember(dest => dest.DoctorFirstName, opt => opt.MapFrom(src => src.Doctor.FirstName))
    .ForMember(dest => dest.DoctorLastName, opt => opt.MapFrom(src => src.Doctor.LastName))
    .ForMember(dest => dest.HospitalName, opt => opt.MapFrom(src => src.HospitalSpecialty.HospitalAsset.Name))
    .ForMember(dest => dest.SpecialtyName, opt => opt.MapFrom(src => src.HospitalSpecialty.Specialty.Name))
    .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.Date));


            // --- Mapping from DTO to Entity for Creation ---
            // This mapping is used when creating a new Schedule entity from an incoming CreateScheduleDto.
            CreateMap<CreateScheduleDto, Schedule>()
                .ForMember(dest => dest.BookedSlots, opt => opt.Ignore()) // Always 0 for new schedules
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // ID is generated by DB
                .ForMember(dest => dest.Doctor, opt => opt.Ignore()) // Nav property not set from DTO
                .ForMember(dest => dest.HospitalSpecialty, opt => opt.Ignore()) // Nav property not set from DTO
             
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.Date)); // Store only the date part


            // --- Mapping from DTO to Entity for Updates ---
            // This mapping is used when updating an existing Schedule entity from an incoming UpdateScheduleDto.
            // It uses Condition to only update properties if they are not null in the DTO (for partial updates).
            CreateMap<UpdateScheduleDto, Schedule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Don't update the ID
                .ForMember(dest => dest.BookedSlots, opt => opt.Ignore()) // BookedSlots managed separately by logic
                .ForMember(dest => dest.Doctor, opt => opt.Ignore()) // Nav property not updated from DTO directly
                .ForMember(dest => dest.HospitalSpecialty, opt => opt.Ignore()) // Nav property not updated from DTO directly
               
                .ForMember(dest => dest.DoctorId, opt => opt.Condition(src => src.DoctorId != null)) // Only update if provided
                .ForMember(dest => dest.HospitalSpecialtyId, opt => opt.Condition(src => src.HospitalSpecialtyId.HasValue)) // Only update if provided
                .ForMember(dest => dest.Date, opt => opt.Condition(src => src.Date.HasValue)) // Only update if provided
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.Value.Date)) // Map date part if present
                .ForMember(dest => dest.StartTime, opt => opt.Condition(src => src.StartTime.HasValue))
                .ForMember(dest => dest.EndTime, opt => opt.Condition(src => src.EndTime.HasValue))
                .ForMember(dest => dest.MaxCapacity, opt => opt.Condition(src => src.MaxCapacity.HasValue))
                .ForMember(dest => dest.IsActive, opt => opt.Condition(src => src.IsActive.HasValue));

        }
    }
}