using Domain.Common.Auth.IdentityAuth;
using Microsoft.AspNetCore.Identity;

namespace Domain.DatingSite
{
    public class AppUser : IdentityUser<int>
    {
        // not needed as we are using IdentityUser
        //public int Id { get; set; }
        //public string UserName { get; set; }
        //public byte[]? PasswordHash { get; set; }
        //public byte[]? PasswordSalt { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string? Gender { get; set; }
        public string? Introduction { get; set; }
        public string? LookingFor { get; set; }
        public string? Interests { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public ICollection<Photo>? Photos { get; set; }

        public ICollection<UserLike> LikedByUsers { get; set; }
        public ICollection<UserLike> LikedUsers { get; set; }

        public ICollection<Message> MessagesSent { get; set; }
        public ICollection<Message> MessagesReceived { get; set; }

        /// <summary>
        /// AppUserRole acts as a join table 
        /// </summary>
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
