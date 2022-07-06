using Domain.DatingSite;
using Microsoft.AspNetCore.Identity;

namespace Domain.Common.Auth.IdentityAuth
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}
