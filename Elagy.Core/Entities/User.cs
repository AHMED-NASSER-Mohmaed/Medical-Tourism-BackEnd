﻿using Microsoft.AspNetCore.Identity;
using System;
using Elagy.Core.Entities;
using Elagy.Core.Enums;

namespace Elagy.Core.Entities
{
    public abstract class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }


        // Email property is inherited from IdentityUser. do not redefine it here.
         
        public string ImageId { get; set; }
        public string ImageURL { get; set; }


        public Gender Gender { get; set; }

        #region
        public string? Address { get; set; } // Full address as a single string
        public string? City { get; set; } // City name, not a complex object
        public int? GovernorateId { get; set; } //governate forgin key
        public Governorate? Governorate { get; set; } // navigation properity

        #endregion



        public string Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }  


        public Status Status { get; set; }
        public UserType UserType { get; set; } // Discriminator for TPH
    }
}