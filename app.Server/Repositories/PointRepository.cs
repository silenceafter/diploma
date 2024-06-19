using app.server.Controllers.Requests;
using app.Server.Models;
using app.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Server.Repositories
{
    public class PointRepository: IPointRepository
    {
        private readonly EcodbContext _context;

        public PointRepository(EcodbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Point>>? GetPointsAll()
        {
            try
            {
                return await _context.Points.ToListAsync();
            }
            catch(Exception ex)
            {
                return null;
            }       
        }

        public async Task<int> RegisterProductReserve(ReceivingProductRequest request, User user)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var productBonus = 0;
                    foreach(var item in request.Products)
                    {
                        var product = await _context.Products.FindAsync(item.Id);
                        productBonus += product.Bonus;                                                
                    }

                    //пользователю не хватает бонусов для покупки товара
                    if (productBonus >= user.Bonuses)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    var bonusesEnd = user.Bonuses - productBonus;

                    //1 создать транзакцию о приобретении товара
                    var userTransaction = new Transaction()
                    {
                        UserId = user.Id,
                        TypeId = 8,
                        Date = DateTime.UtcNow.ToUniversalTime(),
                        BonusesStart = user.Bonuses,
                        BonusesEnd = user.Bonuses
                    };
                    await _context.Transactions.AddAsync(userTransaction);

                    //сохранить
                    await _context.SaveChangesAsync();
                    var userTransactionId = userTransaction.Id;

                    //2 добавить запись о приобретении товара
                    foreach (var item in request.Products)
                    {
                        var product = await _context.Products.FindAsync(item.Id);
                        await _context.ReceivingProducts.AddAsync(new ReceivingProduct()
                        {
                            TransactionId = userTransactionId,
                            ProductId = product.Id,
                            Date = DateTime.UtcNow.ToUniversalTime()
                        });
                    }
                    
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
    }
}
