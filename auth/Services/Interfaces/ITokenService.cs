using auth.DTOs;
using auth.Models;
using Microsoft.AspNetCore.Identity;

namespace auth.Services.Interfaces
{
    public interface ITokenService
    {
        Task<int> AddJwtTokenToBlacklist(BlacklistedToken blacklistedToken);
        Task<string>? GenerateJwtToken(IdentityUser user);
        Task<string>? GetJwtTokenFromHeader();
        Task<bool> ValidateJwtToken(/*string token*/);
    }
}
