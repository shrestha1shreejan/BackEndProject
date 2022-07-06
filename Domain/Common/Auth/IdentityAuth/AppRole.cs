using Microsoft.AspNetCore.Identity;

namespace Domain.Common.Auth.IdentityAuth
{
    public class AppRole : IdentityRole<int>
    {
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}
