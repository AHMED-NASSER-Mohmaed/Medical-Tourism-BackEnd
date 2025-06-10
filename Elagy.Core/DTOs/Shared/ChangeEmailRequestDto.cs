using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Shared
{
    public class ChangeEmailRequestDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }

        [Required]
        public string CurrentPassword { get; set; }
    }
}