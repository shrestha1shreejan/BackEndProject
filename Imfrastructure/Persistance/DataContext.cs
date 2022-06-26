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
        public DbSet<UserLike> Likes { get; set; }

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

        /// <summary>
        /// override method for the migrations work
        /// HasKey(k => new { k.SourceUserId, k.LikedUserId }) creates key for the joint table
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserLike>().HasKey(k => new { k.SourceUserId, k.LikedUserId });

            /// defining relation
            builder.Entity<UserLike>()
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade); // DeleteBehavior.Noaction in case of sql server

            builder.Entity<UserLike>()
                .HasOne(s => s.LikedUser)
                .WithMany(l => l.LikedByUsers)
                .HasForeignKey(s => s.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
