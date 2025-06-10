using System.ComponentModel.DataAnnotations;

namespace Elagy.Core.DTOs.Shared
{
    public class ResetPasswordRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } // Mark as required in validation, but compiler needs nullable or required keyword

        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
 
          
    }
}