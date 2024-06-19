using app.Server.Controllers.Requests;
using app.Server.Models;
using app.Server.Repositories;
using app.Server.Repositories.Interfaces;
using app.Server.Services;
using app.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace app.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReceivingDiscountController : Controller
    {
        private readonly ILogger<ReceivingDiscountController> _logger;
        private readonly EcodbContext _context;
        private readonly IReceivingDiscountRepository _receivingDiscountRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IUserRepository _userRepository;

        public ReceivingDiscountController(
            ILogger<ReceivingDiscountController> logger,
            EcodbContext context,
            IReceivingDiscountRepository receivingDiscountRepository,
            IEncryptionService encryptionService,
            IUserRepository userRepository
            )
        {
            _logger = logger;
            _context = context;
            _receivingDiscountRepository = receivingDiscountRepository;
            _encryptionService = encryptionService;
            _userRepository = userRepository;
        }

        [HttpPost("register-discount-reserve")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> RegisterDiscountReserve([FromBody] ReceivingDiscountRequest request)
        {
            try
            {
                //пользователь
                var emailHash = _encryptionService.ComputeHash(request.Email);
                var user = await _userRepository.GetUserByEmail(emailHash);

                //пользователь не найден
                if (user == null)
                    return BadRequest();

                var data = await _receivingDiscountRepository.RegisterDiscountReserve(request, user);
                return data > 0 ? Ok(data) : BadRequest();
            }
            catch(Exception ex)
            {
                return BadRequest();
            }            
        }

        [HttpGet("get-discounts")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> GetDiscounts()
        {
            var data = await _receivingDiscountRepository.GetDiscountsAll();
            return Ok(data);
        }
    }
}
