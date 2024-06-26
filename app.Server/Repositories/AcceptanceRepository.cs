﻿using app.Server.Controllers.Requests;
using app.Server.Models;
using app.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace app.Server.Repositories
{
    public class AcceptanceRepository : IAcceptanceRepository
    {
        private readonly EcodbContext _context;

        public AcceptanceRepository(EcodbContext context)
        {
            _context = context;
        }

        public async Task<int> RegisterDispose(List<AcceptanceRequest> request)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    int addedRows = 0;
                    int bonuses = 0;
                    var user = await _context.Users.FindAsync(request[0].UserId);

                    //1 создать транзакцию приема отходов
                    var userTransaction = new Transaction()
                    {
                        UserId = user.Id,
                        TypeId = 1,
                        Date = DateTime.UtcNow.ToUniversalTime(),
                        BonusesStart = user.Bonuses,
                        BonusesEnd = user.Bonuses
                    };
                    await _context.Transactions.AddAsync(userTransaction);

                    //сохранить
                    await _context.SaveChangesAsync();
                    var userTransactionId = userTransaction.Id;

                    foreach (var item in request)
                    {                        
                        //2 добавить запись о приеме отходов
                        await _context.Acceptances.AddAsync(new Acceptance()
                        {
                            TransactionId = userTransactionId,
                            HazardousWasteId = item.HazardousWasteId,
                            Date = DateTime.UtcNow.ToUniversalTime()
                        });
                        
                        //считаем общее количество бонусов
                        var hazardousWaste = await _context.HazardousWastes.FindAsync(item.HazardousWasteId);
                        bonuses += hazardousWaste.Bonuses;                        

                        //количество добавленных строк
                        addedRows += _context.ChangeTracker.Entries().Count(e => e.State == EntityState.Added);
                    }

                    //сохранить
                    await _context.SaveChangesAsync();

                    //3 создать транзакцию начисления бонусов
                    userTransaction = new Transaction()
                    {
                        UserId = user.Id,
                        TypeId = 3,
                        Date = DateTime.UtcNow.ToUniversalTime(),
                        BonusesStart = user.Bonuses,
                        BonusesEnd = user.Bonuses + bonuses
                    };
                    await _context.Transactions.AddAsync(userTransaction);

                    //4 начислить бонусы пользователю                    
                    user.Bonuses += bonuses;

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

        public bool UpdateDispose()
        {
            throw new NotImplementedException();
        }

        public bool DeleteDispose()
        {
            throw new NotImplementedException();
        }

        public async Task<Acceptance>? GetDispose(int id)
        {
            return await _context.Acceptances
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Acceptance>>? GetDisposeAll()
        {
            return await _context.Acceptances.ToListAsync();
            /*return await _context.WasteDisposals
                .Include(p => p.HazardousWaste)
                .ToListAsync();*/
        }
    }
}
