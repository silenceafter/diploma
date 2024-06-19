using app.Server.Controllers.Requests;
using app.Server.Models;
using app.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Server.Repositories
{
    public class ReceivingDiscountRepository : IReceivingDiscountRepository
    {
        private readonly EcodbContext _context;

        public ReceivingDiscountRepository(EcodbContext context)
        {
            _context = context;
        }

        public async Task<int> RegisterDiscountReserve(ReceivingDiscountRequest request, User user)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var discount = await _context.Discounts.FindAsync(request.DiscountId);
                    var bonusesEnd = user.Bonuses - discount.Bonuses;

                    //пользователю не хватает бонусов для покупки купона
                    if (bonusesEnd < 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    //1 создать транзакцию о приобретении купона
                    var userTransaction = new Transaction()
                    {
                        UserId = user.Id,//request.UserId,
                        TypeId = 2,
                        Date = DateTime.UtcNow.ToUniversalTime(),
                        BonusesStart = user.Bonuses,
                        BonusesEnd = user.Bonuses
                    };
                    await _context.Transactions.AddAsync(userTransaction);

                    //сохранить
                    await _context.SaveChangesAsync();
                    var userTransactionId = userTransaction.Id;

                    //2 добавить запись о приобретении купона
                    await _context.ReceivingDiscounts.AddAsync(new ReceivingDiscount()
                    {
                        TransactionId = userTransactionId,
                        DiscountId = request.DiscountId,
                        Date = DateTime.UtcNow.ToUniversalTime()
                    });

                    //3 создать пользовательскую транзакцию списания бонусов
                    userTransaction = new Transaction()
                    {
                        UserId = user.Id,//request.UserId,
                        TypeId = 4,
                        Date = DateTime.UtcNow.ToUniversalTime(),
                        BonusesStart = user.Bonuses,
                        BonusesEnd = bonusesEnd
                    };
                    await _context.Transactions.AddAsync(userTransaction);

                    //4 обновить количество бонусов у пользователя  
                    user.Bonuses = bonusesEnd;

                    //количество добавленных строк
                    int addedRows = _context.ChangeTracker.Entries().Count(e => e.State == EntityState.Added);                    

                    //сохранить
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                    return addedRows;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return 0;
                }
            }
        }

        public async Task<IEnumerable<Discount>>? GetDiscountsAll()
        {
            return await _context.Discounts
                .Include(p => p.Partner)
                .ToListAsync();
        }
    }
}
