//using auth.Areas.Identity.Pages.Account;
using auth.DTOs;
using auth.Models;
using auth.Services;
using auth.Services.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System.Web.Helpers;

namespace auth.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly EmailService _emailService;
        //private readonly IUrlHelper _urlHelper;

        public AccountController(
            UserManager<IdentityUser> userManager, 
            IAccountService accountService, 
            ITokenService tokenService,
            EmailService emailService/*,
            IUrlHelperFactory urlHelperFactory*/)
        {
            _userManager = userManager;
            _accountService = accountService;
            _tokenService = tokenService;
            _emailService = emailService;
            //_urlHelper = urlHelperFactory.GetUrlHelper(new ActionContext());
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            try
            {
                //вход пользователя в систему
                var result = await _accountService.Login(model);
                if (result.Count > 0)
                {
                    var errors = result;
                    ModelState.AddModelError("EmailPassword", "Email или пароль");
                    return BadRequest(new { errors });
                }

                //найти пользователя в системе
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("User", "Пользователь не найден");
                    var errors = new Dictionary<string, string[]>
                    {
                        { "User", new string[] { "Пользователь не найден" } }
                    };
                    return BadRequest(new { errors });//return Unauthorized("User not found");
                }

                //роли пользователя
                var roles = await _userManager.GetRolesAsync(user);

                //генерировать токен
                var token = _tokenService.GenerateJwtToken(user);
                if (token == null)
                {
                    ModelState.AddModelError("Token", "Внутренняя ошибка");
                    var errors = new Dictionary<string, string[]>
                    {
                        { "Token", new string[] { "Внутренняя ошибка" } }
                    };
                    return BadRequest(new { errors });//return Unauthorized("Invalid email or password.");
                }
                return Ok(new { UserName = user.UserName, Role = roles[0], Token = token });
            }
            catch (Exception ex) 
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("logout")]
        [AllowAnonymous]//[Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> Logout()
        {
            return await _accountService.Logout()
                ? Ok()
                : BadRequest("Произошла ошибка при обработке токена");
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            //
            var result = await _accountService.Register(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
               
            //присваиваем пользователю роль "User"
            var roleResult = await _accountService.AssignRoleToUser(model.Email, "User");
            if (!roleResult.Succeeded)
            {
                //если присвоение роли не удалось, откатываем регистрацию пользователя
                await _accountService.DeleteUserByEmail(model.Email);
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return BadRequest(ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }
            return Created("", new { });//201 Created           
        }

        [HttpPost("forgot")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                BadRequest();
            //
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var urlHelper = new UrlHelper(ControllerContext);
            var callbackUrl = urlHelper.Action("ResetPassword", "Account", new { token, model.Email }, urlHelper.ActionContext.HttpContext.Request.Scheme);

            //отправить email с ссылкой на сброс пароля
            await _emailService.SendEmailAsync(model.Email, "Reset Password", $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>");
            return Ok(new { message = "Отправлена ссылка на сброс пароля пользователя" });
        }

        [HttpPost("validate")]
        //[Authorize]
        public async Task<IActionResult> Validate()
        {
            if (await _tokenService.ValidateJwtToken())
                return Ok(true);
            return Unauthorized();
        }

        [HttpGet("get-current-user")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            { 
                var user = await _accountService.GetIdentityUser();
                return Ok(user);
            }
            catch (Exception ex) 
            {
                return BadRequest();
            }
        }

        [HttpGet("get-current-role")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> GetRole()
        {
            var user = await _accountService.GetIdentityUser();
            if (user != null)
                return Ok(user.Roles);
            return BadRequest(
                IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = $"Ошибка предоставления сведений о роли пользователю"
                    }
                )
            );
        }

        [HttpGet("get-roles")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _accountService.GetRoles();
            return Ok(roles);
        }

        [HttpPost("update-role")]
        [Authorize]
        public async Task<IActionResult> UpdateRole(string email, string role)
        {
            var result = await _accountService.AssignRoleToUser(email, role);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(
                IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = $"Ошибка присвоения новой роли пользователю"
                    }
                )
            );
        }                        
    }
}