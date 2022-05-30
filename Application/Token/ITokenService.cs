using Domain.DatingSite;

namespace Application.Token
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
