﻿using Domain.DatingSite;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interface
{
    public interface IDbContext
    {
        public DbSet<AppUser> Users { get; set; }
        Task<int> SaveChangesAsync();
        Task MigrateAsync();
        void Update(AppUser user);
    }
}
