using Application.Common.Interface;
using Domain.DatingSite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    internal sealed class DataContext : DbContext, IDbContext
    {
        #region Constructor
      
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        #endregion

        public DbSet<AppUser> Users { get; set; }

        public async Task MigrateAsync()
        {
             await base.Database.MigrateAsync();
        }

        public async Task<int> SaveChangesAsync()
        {            
            return await base.SaveChangesAsync();
        }

        public void Update(AppUser user)
        {
            base.Entry(user).State = EntityState.Modified;
        }
    }
}
