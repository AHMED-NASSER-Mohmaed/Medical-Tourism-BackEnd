using Elagy.Core.Helpers;
using Microsoft.AspNetCore.Identity; // Using Identity's hasher for consistency

namespace Elagy.BL.Helpers
{
    // This implementation uses Identity's built-in hasher, but exposes it via your interface
    public class PasswordHasher : IPasswordHasher
    {
        private readonly IPasswordHasher<object> _identityPasswordHasher; // Use object for generic hasher

        public PasswordHasher(IPasswordHasher<object> identityPasswordHasher)
        {
            _identityPasswordHasher = identityPasswordHasher;
        }

        public string HashPassword(string password)
        {
            return _identityPasswordHasher.HashPassword(null, password); // Pass null for user object
        }

        public bool VerifyHashedPassword(string hashedPassword, string password)
        {
            var result = _identityPasswordHasher.VerifyHashedPassword(null, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}