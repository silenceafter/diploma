﻿using app.Server.Controllers.Requests;
using app.Server.Models;

namespace app.Server.Repositories.Interfaces
{
    public interface IHazardClassRepository
    {
        public Task<IEnumerable<HazardClass>>? GetHazardClassAll();
    }
}
