using app.Server.Controllers.Requests;
using app.Server.Models;
using app.Server.Repositories;
using app.Server.Repositories.Interfaces;
using app.Server.Services;
using app.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

namespace app.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AcceptanceController : Controller
    {
        private readonly ILogger<AcceptanceController> _logger;
        private readonly EcodbContext _context;
        private readonly IAcceptanceRepository _acceptanceRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICAuthorizationService _authorizationService;

        public AcceptanceController(
            ILogger<AcceptanceController> logger, 
            EcodbContext context,
            IAcceptanceRepository acceptanceRepository,
            IEncryptionService encryptionService,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,
            ICAuthorizationService authorizationService)
        {
            _logger = logger;
            _context = context;
            _acceptanceRepository = acceptanceRepository;
            _encryptionService = encryptionService;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
        }

        [HttpPost("register-dispose")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> RegisterDispose([FromBody] AcceptanceRequest request)
        {
            /* email в request для роли оператора */
            try
            {
                //извлечь информацию из токена
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                //данные сервера авторизации
                var authorizationData = await _authorizationService.GetAuthorizationData(token);

                //пользователь
                var emailHash = _encryptionService.ComputeHash(authorizationData.Email);
                var user = await _userRepository.GetUserByEmail(emailHash);

                //пользователь не найден
                if (user == null)
                    return BadRequest();

                //регистрируем прием
                var data = await _acceptanceRepository.RegisterDispose(request, user);
                return Ok(data);
            }
            catch(Exception ex)
            {
                return BadRequest();
            }            
        }   
    }
}