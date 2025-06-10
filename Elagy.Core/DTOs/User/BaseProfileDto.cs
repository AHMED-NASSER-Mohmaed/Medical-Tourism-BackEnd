using Elagy.Core.Enums;
using System;

namespace Elagy.Core.DTOs.User
{
    public abstract class BaseProfileDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; } // Include from IdentityUser
        public string Nationality { get; set; }
        public string NationalId { get; set; }
        public string PassportId { get; set; }
        public string ImageId { get; set; }
        public string ImageURL { get; set; }
        public string Gender { get; set; }
        public UserStatus Status { get; set; }
        public string ZipCode { get; set; }
        public string StreetNumber { get; set; }
        public string Governorate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public UserType UserType { get; set; } // Discriminator to know the actual type
        public DateTimeOffset? LockoutEnd { get; set; } // From IdentityUser, useful for dashboard
        public bool EmailConfirmed { get; set; } // From IdentityUser
    }
}