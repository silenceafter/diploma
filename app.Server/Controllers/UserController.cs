using app.Server.Controllers.Requests;
using app.Server.Controllers.Response;
using app.Server.Models;
using app.Server.Repositories;
using app.Server.Repositories.Interfaces;
using app.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace app.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly EcodbContext _context;
        private readonly IEncryptionService _encryptionService;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SettingsJwt _settingsJwt;
        private readonly ITokenValidationService _tokenValidationService;
        private readonly ICAuthorizationService _authorizationService;

        public UserController(
            ILogger<UserController> logger,
            EcodbContext context,
            IEncryptionService encryptionService,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            IOptions<SettingsJwt> settingsJwt,
            ITokenValidationService tokenValidationService,
            ICAuthorizationService authorizationService
            )
        {
            _logger = logger;
            _context = context;
            _encryptionService = encryptionService;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _settingsJwt = settingsJwt.Value;
            _tokenValidationService = tokenValidationService;
            _authorizationService = authorizationService;
        }

        [HttpGet("get-account-balance")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> GetAccountBalance()
        {
            try
            {
                //извлечь информацию из токена
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                //данные сервера авторизации
                var authorizationData = await _authorizationService.GetAuthorizationData(token);
                var encrypt = _encryptionService.Encrypt(authorizationData.Email);
                var emailHash = _encryptionService.ComputeHash(authorizationData.Email);
                //
                var user = await _userRepository.GetUserByEmail(emailHash);
                if (user == null)
                    return BadRequest();
                return Ok(new { bonus = user.Bonuses });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("get-user")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> GetUser([FromBody] UserRequest request)
        {
            try
            {
                var encrypt = _encryptionService.Encrypt(request.Email);
                var emailHash = _encryptionService.ComputeHash(request.Email);
                //
                var user = await _userRepository.GetUserByEmail(emailHash);
                if (user != null)//токен опознан
                    return Ok(new UserResponse() { Bonus = user.Bonuses, Role = "Пользователь" });
                return Ok(null);
            }
            catch (Exception ex)
            {
                return Ok(null);
            }
        }

        [HttpGet("get-current-user")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                //извлечь информацию из токена
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                //данные сервера авторизации
                var authorizationData = await _authorizationService.GetAuthorizationData(token);

                //данные ecodb
                var emailHash = _encryptionService.ComputeHash(authorizationData.Email);      
                var user = await _userRepository.GetUserByEmail(emailHash);
                return Ok(new UserResponse() 
                { 
                    UserName = authorizationData.UserName , 
                    Email = authorizationData.Email, 
                    Bonus = user.Bonuses, 
                    Role = authorizationData.Roles[0] 
                });
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("get-user-transactions")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> GetUserTransactions()
        {
            try
            {
                //извлечь информацию из токена
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                //данные сервера авторизации
                var authorizationData = await _authorizationService.GetAuthorizationData(token);

                //данные ecodb
                var emailHash = _encryptionService.ComputeHash(authorizationData.Email);
                var user = await _userRepository.GetUserByEmail(emailHash);
                var transactions = await _userRepository.GetTransactionByUserId(user.Id);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("get-user-acceptance")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> GetUserAcceptance()
        {
            try
            {
                //извлечь информацию из токена
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                //данные сервера авторизации
                var authorizationData = await _authorizationService.GetAuthorizationData(token);

                //данные ecodb
                var emailHash = _encryptionService.ComputeHash(authorizationData.Email);
                var user = await _userRepository.GetUserByEmail(emailHash);
                var transactions = await _userRepository.GetAcceptanceByUserId(user.Id);//inner join транзакции приема отходов
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("get-user-receiving-discount")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> GetUserReceivingDiscounts()
        {
            try
            {
                //извлечь информацию из токена
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                //данные сервера авторизации
                var authorizationData = await _authorizationService.GetAuthorizationData(token);

                //данные ecodb
                var emailHash = _encryptionService.ComputeHash(authorizationData.Email);
                var user = await _userRepository.GetUserByEmail(emailHash);
                var transactions = await _userRepository.GetReceivingDiscountsByUserId(user.Id);//inner join транзакции приема отходов
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("get-user-receiving-product")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> GetUserReceivingProducts()
        {
            try
            {
                //извлечь информацию из токена
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                //данные сервера авторизации
                var authorizationData = await _authorizationService.GetAuthorizationData(token);

                //данные ecodb
                var emailHash = _encryptionService.ComputeHash(authorizationData.Email);
                var user = await _userRepository.GetUserByEmail(emailHash);
                var transactions = await _userRepository.GetReceivingProductByUserId(user.Id);//inner join транзакции обмена бонусов на товары
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet("login")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> Login()
        {
            try
            {
                //извлечь информацию из токена
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var userResponse = _tokenValidationService.GetPayloadData(token);

                var encrypt = _encryptionService.Encrypt(userResponse.Email);
                var emailHash = _encryptionService.ComputeHash(userResponse.Email);
                //
                var user = await _userRepository.GetUserByEmail(emailHash);
                if (user != null)//токен опознан
                    return Ok(true);

                //пользователь не найден -> регистрируем по email в зашифрованном виде
                var response = await _userRepository.Create(encrypt, emailHash);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }            
        }
    }
}