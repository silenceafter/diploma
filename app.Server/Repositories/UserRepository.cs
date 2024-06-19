using app.server.Controllers.Response;
using app.Server.Controllers.Requests;
using app.Server.Models;
using app.Server.Repositories.Interfaces;
using app.Server.TempModels;
using Microsoft.EntityFrameworkCore;

namespace app.Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EcodbContext _context;

        public UserRepository(EcodbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(string encrypt, string emailHash)
        {
            await using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //добавить пользователя
                    var user = new User() { Encrypt = encrypt, EmailHash = emailHash, RoleId = 2 };
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    //добавить транзакцию
                    var userTransaction = new Transaction()
                    {
                        UserId = user.Id,
                        TypeId = 5,
                        Date = DateTime.UtcNow.ToUniversalTime(),
                        BonusesStart = user.Bonuses,
                        BonusesEnd = user.Bonuses
                    };
                    await _context.Transactions.AddAsync(userTransaction);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return user.Id;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return 0;
                }
            }
            return 0;
        }

        public bool Update()
        {
            return true;
        }

        public bool Delete()
        {
            return true;
        }

        public Task<User>? GetUserById(int id)
        {
            return null;
        }

        public async Task<User>? GetUserByEmail(string emailHash)
        {            
            return await _context.Users.FirstOrDefaultAsync(u => u.EmailHash == emailHash);
        }

        public async Task<List<TransactionResponse>>? GetTransactionByUserId(int userId)
        {
            try
            {
                var transactions = await _context.Transactions
                    .Include(u => u.Type)
                    .Where(u => u.UserId == userId)
                    .OrderBy(u => u.Id)
                    .ToListAsync();
                //
                var response = new List<TransactionResponse>();
                foreach(var transaction in transactions)
                {
                    response.Add(new TransactionResponse()
                    {
                        Id = transaction.Id,
                        TransactionName = transaction.Type.Name,
                        Date = transaction.Date,
                        BonusesStart = transaction.BonusesStart,
                        BonusesEnd = transaction.BonusesEnd
                    });
                }
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }            
        }

        public async Task<List<TransactionResponse>>? GetTransactionById(int id)
        {
            try
            {
                var transactions = await _context.Transactions
                    .Include(u => u.Type)
                    .Where(u => u.Id == id)
                    .OrderBy(u => u.Id)
                    .ToListAsync();
                //
                var response = new List<TransactionResponse>();
                foreach (var transaction in transactions)
                {
                    response.Add(new TransactionResponse()
                    {
                        Id = transaction.Id,
                        TransactionName = transaction.Type.Name,
                        Date = transaction.Date,
                        BonusesStart = transaction.BonusesStart,
                        BonusesEnd = transaction.BonusesEnd
                    });
                }
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<AcceptanceResponse>>? GetAcceptanceByUserId(int userId)
        {
            try
            {
                var transactions = await _context.Transactions
                    .Where(t => t.UserId == userId && t.TypeId == 1)
                    .Include(u => u.Type)
                    .Join(
                        _context.Acceptances,
                        transaction => transaction.Id,
                        acceptance => acceptance.TransactionId,
                        (transaction, acceptance) => new
                        {
                            Transaction = transaction,
                            Acceptance = acceptance
                        }
                    )
                    .Join(
                        _context.HazardousWastes,
                        combined => combined.Acceptance.HazardousWasteId,
                        hazardousWaste => hazardousWaste.Id,
                        (combined, hazardousWaste) => new
                        {
                            combined.Transaction,
                            combined.Acceptance,
                            HazardousWasteName = hazardousWaste.Name
                        }
                    )
                    .Join(
                        _context.Points,
                        combined => combined.Acceptance.PointId,
                        point => point.Id,
                        (combined, point) => new
                        {
                            combined.Transaction,
                            combined.Acceptance,
                            combined.HazardousWasteName,
                            PointName = point.Name,
                            PointAddress = point.Address
                        }
                    )
                    .Select(result => new
                    {
                        TransactionId = result.Transaction.Id,
                        HazardousWasteName = result.HazardousWasteName,
                        Quantity = result.Acceptance.Quantity,
                        PointName = result.PointName,
                        PointAddress = result.PointAddress,
                        Date = result.Acceptance.Date
                    })
                    .OrderBy(result => result.TransactionId)
                    .ToListAsync();
                //
                var response = new List<AcceptanceResponse>();
                foreach (var transaction in transactions)
                {
                    response.Add(new AcceptanceResponse()
                    {
                        Id = transaction.TransactionId,
                        HazardousWasteName = transaction.HazardousWasteName,
                        Quantity = transaction.Quantity,
                        PointName = transaction.PointName,
                        PointAddress = transaction.PointAddress,
                        Date = transaction.Date
                    });
                }
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ReceivingDiscountsResponse>>? GetReceivingDiscountsByUserId(int userId)
        {
            try
            {
                var transactions = await _context.Transactions
                    .Where(t => t.UserId == userId && t.TypeId == 2)
                     .Include(u => u.Type)
                    .Join(
                        _context.ReceivingDiscounts,
                        transaction => transaction.Id,
                        receivingDiscount => receivingDiscount.TransactionId,
                        (transaction, receivingDiscount) => new
                        {
                            Transaction = transaction,
                            ReceivingDiscount = receivingDiscount
                        }
                    )
                    .Join(
                        _context.Discounts,
                        combined => combined.ReceivingDiscount.DiscountId,
                        discount => discount.Id,
                        (combined, discount) => new
                        {
                            combined.Transaction,
                            combined.ReceivingDiscount,
                            Discount = discount,
                            PartnerId = discount.PartnerId
                        }
                    )
                    .Join(
                        _context.Partners,
                        combined => combined.PartnerId,
                        partner => partner.Id,
                        (combined, partner) => new
                        {
                            combined.Transaction,
                            combined.ReceivingDiscount,
                            combined.Discount,
                            PartnerName = partner.Name
                        }
                    )                   
                    .Select(result => new
                    {
                        TransactionId = result.Transaction.Id,
                        PartnerName = result.PartnerName,
                        DiscountTerms = result.Discount.Terms,
                        DiscountDateStart = result.Discount.DateStart,
                        DiscountDateEnd = result.Discount.DateEnd,
                        DiscountBonuses = result.Discount.Bonuses,
                        Date = result.ReceivingDiscount.Date
                    })
                    .OrderBy(result => result.TransactionId)
                    .ToListAsync();
                //
                var response = new List<ReceivingDiscountsResponse>();
                foreach (var transaction in transactions)
                {
                    response.Add(new ReceivingDiscountsResponse()
                    {
                        Id = transaction.TransactionId,
                        PartnerName = transaction.PartnerName,
                        DiscountTerms = transaction.DiscountTerms,
                        DiscountDateStart = transaction.DiscountDateStart,
                        DiscountDateEnd = transaction.DiscountDateEnd,
                        DiscountBonuses = transaction.DiscountBonuses,
                        Date = transaction.Date
                    });
                }
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<ReceivingProductResponse>>? GetReceivingProductByUserId(int userId)
        {
            try
            {
                var transactions = await _context.Transactions
                    .Where(t => t.UserId == userId && t.TypeId == 8)
                     .Include(u => u.Type)
                    .Join(
                        _context.ReceivingProducts,
                        transaction => transaction.Id,
                        receivingProduct => receivingProduct.TransactionId,
                        (transaction, receivingProduct) => new
                        {
                            Transaction = transaction,
                            ReceivingProduct = receivingProduct
                        }
                    )
                    .Join(
                        _context.Products,
                        combined => combined.ReceivingProduct.ProductId,
                        product => product.Id,
                        (combined, product) => new
                        {
                            combined.Transaction,
                            combined.ReceivingProduct,
                            Product = product,
                            ProductName = product.Name,
                            ProductBonus = product.Bonus
                        }
                    )                   
                    .Select(result => new
                    {
                        TransactionId = result.Transaction.Id,
                        ProductName = result.ProductName,
                        ProductQuantity = result.ReceivingProduct.Quantity,
                        ProductBonus = result.ProductBonus,
                        Date = result.ReceivingProduct.Date
                    })
                    .OrderBy(result => result.TransactionId)
                    .ToListAsync();
                //
                var response = new List<ReceivingProductResponse>();
                foreach (var transaction in transactions)
                {
                    response.Add(new ReceivingProductResponse()
                    {
                        Id = transaction.TransactionId,
                        ProductName = transaction.ProductName,
                        ProductQuantity = transaction.ProductQuantity,
                        ProductBonus = transaction.ProductBonus,
                        Cost = transaction.ProductBonus * transaction.ProductQuantity,
                        Date = transaction.Date
                    });
                }
                return response;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
