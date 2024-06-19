using auth.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using auth.DTOs;
using Microsoft.Extensions.Options;
using auth.Data;
using auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace auth.Services
{
    public class TokenService : ITokenService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TokenService> _logger;
        private readonly SettingsJwtDto _settingsJwtDto;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public TokenService(
            ILogger<TokenService> logger, 
            IHttpContextAccessor httpContextAccessor, 
            IOptions<SettingsJwtDto> settingsJwtDto,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _settingsJwtDto = settingsJwtDto.Value;
            _context = context;
            _userManager = userManager;
        }

        public async Task<int> AddJwtTokenToBlacklist(BlacklistedToken blacklistedToken)
        {
            await using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.BlacklistedTokens.AddAsync(blacklistedToken);                    
                    var addedRows = _context.ChangeTracker.Entries().Count(e => e.State == EntityState.Added);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return addedRows;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return 0;
                }
            }
        }

        public async Task<string>? GenerateJwtToken(IdentityUser user)
        {
            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                /*var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                };*/
                //
                
                
                //создание токена JWT
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_settingsJwtDto.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] 
                    { 
                        new Claim(ClaimTypes.Name, user.Id),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                    }
                    ), // Используем клеймы текущего пользователя
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_settingsJwtDto.ExpirationMinutes)),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _settingsJwtDto.Issuer,//"https://localhost:7086/"
                    Audience = _settingsJwtDto.Audience
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);

                /*var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settingsJwtDto.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _settingsJwtDto.Issuer,
                    audience: _settingsJwtDto.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_settingsJwtDto.ExpirationMinutes)),
                    signingCredentials: creds);

                return new JwtSecurityTokenHandler().WriteToken(token);*/
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token.");
                return null;
            }
        }
    
        public async Task<string>? GetJwtTokenFromHeader()
        {
            try
            {
                return _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> ValidateJwtToken()
        {
            try
            { 
                //извлечь токен
                var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ").Last();
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_settingsJwtDto.SecretKey);
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settingsJwtDto.Issuer,
                    ValidAudience = _settingsJwtDto.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                //валидируем токен
                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                //извлекаем клеймы из токена
                var jwtToken = validatedToken as JwtSecurityToken;
                var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
                var email = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
                var role = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role")?.Value;

                //нет основных клеймов => валидация не пройдена
                if (userId == null || email == null || role == null)
                    return false;

                //проверить в списке отозванных
                var blacklistedToken = await _context.BlacklistedTokens.FirstOrDefaultAsync(u => u.Token == token);
                if (blacklistedToken != null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> ValidateRoles(IdentityUser user, List<string> roles)
        {
            foreach (var role in roles) 
            {
                if (!await _userManager.IsInRoleAsync(user, role))
                    return false;
            }
            return true;
        }
    }
}