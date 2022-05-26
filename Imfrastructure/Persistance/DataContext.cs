using Application.Common.Interface;
using Domain.DatingSite;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    internal sealed  class DataContext : DbContext, IDbContext
    {
        #region Constructor

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        #endregion

        public DbSet<AppUser> Users { get; set; }
    }
}
