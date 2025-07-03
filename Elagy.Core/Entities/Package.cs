using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elagy.Core.Entities
{
    internal class Package
    {
         public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }// Assuming these are DateTimeOffset for better timezone handling



        public string PatientId { get; set; } // Assuming Patient.Id is a string (from IdentityUser)

        //naavigation property
        public Patient Patient { get; set; }


        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();


    }
}
