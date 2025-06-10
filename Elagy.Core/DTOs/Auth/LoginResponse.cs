using Elagy.Core.Enums;

namespace Elagy.Core.DTOs.Auth
{
    public class LoginResponse
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string UserId { get; set; }
        public UserType? UserType { get; set; }
    }

}