using app.Server.Controllers.Requests;
using app.Server.Controllers.Response;

namespace app.Server.Services.Interfaces
{
    public interface ITokenValidationService
    {
        UserResponse GetPayloadData(string token);
        Task<bool> Validate(string token);
    }
}
