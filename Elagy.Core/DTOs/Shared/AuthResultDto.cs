using System.Collections.Generic;

namespace Elagy.Core.DTOs.Shared
{
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string Token { get; set; } // JWT token for successful logins
        public string UserId { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Errors { get; set; }

        public AuthResultDto()
        {
            Errors = new List<string>();
        }
    }
}