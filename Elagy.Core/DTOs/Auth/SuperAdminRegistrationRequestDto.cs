using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Auth
{
    public class SuperAdminRegistrationRequestDto : BaseRegistrationRequestDto
    {
        public string Docs { get; set; }
    }
}