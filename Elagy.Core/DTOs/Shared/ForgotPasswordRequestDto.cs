using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Shared
{
    public class ForgotPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}