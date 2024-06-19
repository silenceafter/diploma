using app.server.Controllers.Response;
using app.Server.Controllers.Requests;
using app.Server.Models;

namespace app.Server.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<int> Create(string encrypt, string emailHash);
        public bool Update();
        public bool Delete();
        public Task<User>? GetUserById(int id);
        public Task<User>? GetUserByEmail(string email);
        //public Task<IEnumerable<Acceptance>>? GetDisposeAll();

        public Task<List<TransactionResponse>>? GetTransactionByUserId(int userId);//все транзакции пользователя по id пользователя
        public Task<List<TransactionResponse>>? GetTransactionById(int id);//все транзакции пользователя по id типа транзакции
        public Task<List<AcceptanceResponse>>? GetAcceptanceByUserId(int userId);
        public Task<List<ReceivingDiscountsResponse>>? GetReceivingDiscountsByUserId(int userId);
        public Task<List<ReceivingProductResponse>>? GetReceivingProductByUserId(int userId);
        
    }
}
