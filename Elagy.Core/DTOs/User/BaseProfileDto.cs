using Elagy.Core.Enums;
using System;

namespace Elagy.Core.DTOs.User
{
    public abstract class BaseProfileDto
    {
        public string Id { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }


        public string PhoneNumber { get; set; } 

        public string ImageURL { get; set; }

        public string Gender { get; set; }


        #region //address properties  
        public string Address { get; set; } // Full address as a single string
        public string City { get; set; } // City name, not a complex object
        public Governorate Governorate { get; set; }
        public Country Country { get; set; } // Country name, not a complex object
        #endregion



        public DateTime? DateOfBirth { get; set; }


        public bool EmailConfirmed { get; set; } // From IdentityUser
        public UserStatus Status { get; set; }
        public UserType UserType { get; set; } // Discriminator to know the actual type

    }
}