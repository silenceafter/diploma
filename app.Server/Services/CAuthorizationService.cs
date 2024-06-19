using app.Server.Controllers.Response;
using app.Server.Models;
using app.Server.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Net.Http.Headers;

namespace app.Server.Services
{
    public class CAuthorizationService : ICAuthorizationService
    {
        private readonly HttpClient _httpClient;
        private readonly SettingsJwt _settingsJwt;

        public CAuthorizationService(HttpClient httpClient, IOptions<SettingsJwt> settingsJwt)
        {
            _httpClient = httpClient;
            _settingsJwt = settingsJwt.Value;
        }

        public async Task<AuthorizationResponse>? GetAuthorizationData(string token)
        {
            //получить данные пользователя от сервиса авторизации
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync("https://localhost:7086/account/get-current-user");
                response.EnsureSuccessStatusCode();
                //
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<AuthorizationResponse>(json, options);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
