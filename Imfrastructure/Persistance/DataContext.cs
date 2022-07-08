using Domain.Common.Auth.IdentityAuth;
using Domain.DatingSite;
using Domain.DatingSite.TrackingEntities;
using Infrastructure.Persistance.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance
{
    /// <summary>
    /// we have to identity each type needed to add to identity
    /// </summary>
    public class DataContext : IdentityDbContext<
        AppUser,
        AppRole, 
        int,
        IdentityUserClaim<int>, 
        AppUserRole, 
        IdentityUserLogin<int>, 
        IdentityRoleClaim<int>, 
        IdentityUserToken<int>
        >   , IDbContext    
    {
        #region Constructor
      
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        #endregion

        // Removing this as Identity will do this for us      
        public DbSet<Photo> Photos { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        // Group user tracking
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }
        //

        public async Task MigrateAsync()
        {
             await base.Database.MigrateAsync();
        }

        public async Task<int> SaveChangesAsync()
        {            
            var result = await base.SaveChangesAsync();
            return result;
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

            /// for identity (configure relation between AppUser and AppRole
            builder.Entity<AppUser>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            builder.Entity<AppRole>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            /////


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

            /// message config
            builder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessagesReceived)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(u => u.Sender)
                .WithMany(m => m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
