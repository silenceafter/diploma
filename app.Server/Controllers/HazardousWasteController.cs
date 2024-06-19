using app.Server.Controllers.Requests;
using app.Server.Models;
using app.Server.Repositories;
using app.Server.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace app.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HazardousWasteController : Controller
    {
        private readonly ILogger<HazardousWasteController> _logger;
        private readonly EcodbContext _context;
        private readonly IHazardousWasteRepository _hazardousWasteRepository;

        public HazardousWasteController(
            ILogger<HazardousWasteController> logger,
            EcodbContext context,
            IHazardousWasteRepository hazardousWasteRepository
            )
        {
            _logger = logger;
            _context = context;
            _hazardousWasteRepository = hazardousWasteRepository;
        }

        [HttpGet("get-hazardous-waste")]
        [Authorize(Policy = "AllowIfNoRoleClaim")]
        public async Task<IActionResult> GetHazardousWaste()
        {
            var data = await _hazardousWasteRepository.GetHazardousWasteAll();
            return Ok(data);
        }
    }
}
