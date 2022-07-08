using Domain.DatingSite;
using Domain.DatingSite.TrackingEntities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Interface
{
    public interface IDbContext
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        // Group user tracking
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }
        
        //
        Task<int> SaveChangesAsync();
        Task MigrateAsync();
        void Update(AppUser user);

        bool HasChanges();
    }
}
