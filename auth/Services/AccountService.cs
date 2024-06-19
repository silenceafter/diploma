//using auth.Areas.Identity.Pages.Account;
using auth.Data;
using auth.DTOs;
using auth.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using auth.Models;
using Microsoft.AspNetCore.Mvc;

namespace auth.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SettingsJwtDto _settingsJwtDto;
        private readonly ITokenService _tokenService;

        public AccountService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IOptions<SettingsJwtDto> settingsJwtDto,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _settingsJwtDto = settingsJwtDto.Value;
            _tokenService = tokenService;
        }

        public async Task<IdentityResult> Register(RegisterDto model)
        {
            await using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var user = new IdentityUser 
                    { 
                        UserName = model.Username, 
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber
                    };
                    
                    //создание пользователя
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (!result.Succeeded)
                    {
                        //откат
                        await transaction.RollbackAsync();
                        return result;
                    }                        

                    //сохранить
                    await _context.SaveChangesAsync();

                    //временное подтверждение email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    result = await _userManager.ConfirmEmailAsync(user, token);

                    //сохранить
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return result;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new IdentityResult();
                }
            }
        }

        public async Task<Dictionary<string, string[]>> Login(LoginDto model)
        {
            try
            {
                //пользователь
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {                 
                    return new Dictionary<string, string[]>
                    {
                         { "Email", new string[] { "Email" } },
                    };
                }//пользователь не найден

                //вход по паролю
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    return new Dictionary<string, string[]>
                    {
                        { "Password", new string[] { "Пароль" } },
                    };
                }
                return new Dictionary<string, string[]> { };
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string[]>
                {
                    { "Email", new string[] { "Email" } },
                    { "Password", new string[] { "Пароль" } },
                };
            }
        }

        public async Task<bool> Logout()
        {
            try
            {
                //получить токен
                var token = await _tokenService.GetJwtTokenFromHeader();
                if (token == null)
                    return false;//BadRequest("Произошла ошибка при обработке токена");
                //
                var blacklistedToken = new BlacklistedToken
                {
                    Token = token,
                    ExpirationDate = DateTime.UtcNow.AddHours(1) //срок действия токена
                };
                //
                await _signInManager.SignOutAsync();
                return await _tokenService.AddJwtTokenToBlacklist(blacklistedToken) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }      

        /*public async Task<IdentityResult> Forgot(ForgotDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Если пользователь не существует или не подтвердил свой адрес электронной почты
                //return BadRequest("User does not exist or email not confirmed.");
            }
        }*/

        public async Task<UserDto>? GetIdentityUser()
        {
            try
            {
                //пользователь
                var email = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
                var user = await _userManager.FindByEmailAsync(email);
                var claims = await _userManager.GetClaimsAsync(user);
                if (user == null)
                    return null;

                //роль
                var roles = await _userManager.GetRolesAsync(user);
                var rolesList = new List<string>();
                foreach(var item in roles)
                    rolesList.Add(item);
                //
                return new UserDto() 
                { 
                    UserName = user.UserName, 
                    Email = user.Email, 
                    PhoneNumber = user.PhoneNumber,
                    Roles = rolesList
                };
                }
            catch (Exception ex) 
            {
                return null;
            }
        }

        public async Task<IdentityResult> AssignRoleToUser(string email, string role)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError
                    {
                        Description = $"User with email {email} not found."
                    });
                }

                //проверяем, существует ли роль
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    //создаем роль, если она не существует
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!roleResult.Succeeded)
                        return roleResult;
                }

                //присваиваем пользователю роль
                var result = await _userManager.AddToRoleAsync(user, role);
                return result;
            }
            catch(Exception ex)
            {
                //возвращаем ошибку
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = $"An error occurred while deleting user: {ex.Message}"
                    }
                );
            }
        }
    
        public async Task<IdentityResult> DeleteUserByEmail(string email)
        {
            try
            {
                //находим пользователя по адресу электронной почты
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    //пользователь не найден, возвращаем ошибку
                    return IdentityResult.Failed(
                        new IdentityError 
                        { 
                            Description = $"User with email '{email}' not found." 
                        }
                    );
                }

                //удаляем пользователя
                var result = await _userManager.DeleteAsync(user);
                return result;
            }
            catch(Exception ex)
            {
                //пользователь не найден, возвращаем ошибку
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = $"An error occurred while deleting user: {ex.Message}"
                    }
                );
            }
        }
    
        public async Task<List<RoleDto>> GetRoles()
        {
            try
            {
                var roles = await _roleManager.Roles.ToListAsync();
                var result = new List<RoleDto>();
                foreach(var role in roles)
                {
                    result.Add(
                        new RoleDto
                        {
                            Name = role.Name,
                            NormalizedName = role.NormalizedName
                        }
                    );
                }
                return result;
            }
            catch(Exception ex)
            {
                return new List<RoleDto>();
            }
        }
    }
}