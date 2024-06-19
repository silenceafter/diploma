using app.server.Controllers.Requests;
using app.Server.Controllers.Requests;
using app.Server.Models;

namespace app.Server.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>>? GetProductsAll();
        public Task<int> RegisterProductReserve(ReceivingProductRequest request, User user);
    }
}
