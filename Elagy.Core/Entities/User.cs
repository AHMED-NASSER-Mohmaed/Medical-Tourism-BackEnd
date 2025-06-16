using Elagy.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System;

namespace Elagy.Core.Entities
{
    public abstract class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // Email property is inherited from IdentityUser.
        public string Nationality { get; set; }
        public string ImageId { get; set; }
        public string ImageURL { get; set; }
        public string Gender { get; set; }
        public UserStatus Status { get; set; }
        public string ZipCode { get; set; }
        public string StreetNumber { get; set; }
        public string Governorate { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public UserType UserType { get; set; } // Discriminator for TPH
    }
}