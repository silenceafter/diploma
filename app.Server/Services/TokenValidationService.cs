using app.Server.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Net.Http.Headers;
using app.Server.Controllers.Response;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;

namespace app.Server.Services
{
    public class TokenValidationService : ITokenValidationService
    {
        private readonly HttpClient _httpClient;
        private readonly SettingsJwt _settingsJwt;

        public TokenValidationService(HttpClient httpClient, IOptions<SettingsJwt> settingsJwt)
        {
            _httpClient = httpClient;
            _settingsJwt = settingsJwt.Value;
        }

        public async Task<bool> Validate(string token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync("https://localhost:7086/account/validate", null);
                return response.IsSuccessStatusCode;           
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    
        public UserResponse GetPayloadData(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settingsJwt.SecretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _settingsJwt.Issuer,
                ValidAudience = _settingsJwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            //валидируем токен
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            //извлекаем клеймы из токена
            var jwtToken = validatedToken as JwtSecurityToken;
            var userId = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "unique_name")?.Value;
            var email = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;
            var role = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "role")?.Value;
            return new UserResponse 
            { 
                Id = userId, 
                Email = email, 
                Role = role 
            };
        }
    }
}
