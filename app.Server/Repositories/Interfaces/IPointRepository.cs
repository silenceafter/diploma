using app.server.Controllers.Requests;
using app.Server.Controllers.Requests;
using app.Server.Models;

namespace app.Server.Repositories.Interfaces
{
    public interface IPointRepository
    {
        Task<IEnumerable<Point>>? GetPointsAll();        
    }
}
