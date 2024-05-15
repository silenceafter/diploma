using auth.DTOs;

namespace auth.Services.Interfaces
{
    public interface IAccountService
    {
        Task Register(RegisterDto model);
    }
}
