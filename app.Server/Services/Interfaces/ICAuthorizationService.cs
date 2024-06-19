using app.Server.Controllers.Response;

namespace app.Server.Services.Interfaces
{
    public interface ICAuthorizationService
    {
        Task<AuthorizationResponse>? GetAuthorizationData(string token);
    }
}
