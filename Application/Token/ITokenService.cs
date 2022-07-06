using Domain.DatingSite;

namespace Application.Token
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user);
    }
}
