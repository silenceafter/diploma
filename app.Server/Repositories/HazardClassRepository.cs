﻿using app.Server.Controllers.Requests;
using app.Server.Models;
using app.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Server.Repositories
{
    public class HazardClassRepository : IHazardClassRepository
    {
        private readonly EcodbContext _context;

        public HazardClassRepository(EcodbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HazardClass>>? GetHazardClassAll()
        {
            return await _context.HazardClasses.ToListAsync();
        }
    }
}
