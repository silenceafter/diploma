using app.Server.Controllers.Requests;
using app.Server.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace app.Server.Repositories.Interfaces
{
    public interface IAcceptanceRepository
    {
        public Task<int> RegisterDispose(AcceptanceRequest request, User user);
        public bool UpdateDispose();
        public bool DeleteDispose();
        public Task<Acceptance>? GetDispose(int id);
        public Task<IEnumerable<Acceptance>>? GetDisposeAll();
    }
}
