using Elagy.Core.Entities;

namespace Elagy.Core.Helpers
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateTokenAsync(User user);
    }
}