using auth.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace auth.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> Register(RegisterDto model);
        Task<Dictionary<string, string[]>> Login(LoginDto model);
        Task<bool> Logout();
        //Task<IdentityResult> Forgot(ForgotDto model);
        Task<UserDto>? GetIdentityUser();
        Task<IdentityResult> AssignRoleToUser(string email, string role);
        Task<IdentityResult> DeleteUserByEmail(string email);
        Task<List<RoleDto>> GetRoles();
    }
}
