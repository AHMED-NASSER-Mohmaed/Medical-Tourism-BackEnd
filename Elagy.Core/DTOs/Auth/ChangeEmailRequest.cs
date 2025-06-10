// Path: Elagy.Core/DTOs/Auth/ChangeEmailRequest.cs
using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Auth
{
    public class ChangeEmailRequest
    {
        [Required(ErrorMessage = "New email is required.")]
        [EmailAddress(ErrorMessage = "Invalid new email format.")]
        public string NewEmail { get; set; }

        [Required(ErrorMessage = "Current password is required.")]
        public string CurrentPassword { get; set; }
    }
}