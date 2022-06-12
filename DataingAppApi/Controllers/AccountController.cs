using Application.Common.Interface;
using Application.Token;
using Domain.Common.Auth;
using Domain.DatingSite;
using Domain.DatingSite.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DataingAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IDbContext _dbContext;
        private readonly ITokenService _tokenService;

        #region Constructor

        public AccountController(IDbContext dbContext, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        #endregion

        #region methods

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (registerDto.Username != null && await UserExists(registerDto.Username))
            {
                return BadRequest("Username is already taken");
            }
            // hashing
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            
            return Ok(GetUserDto(user));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login (LoginDto loginDto)
        {
            var user = await _dbContext.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null)
            {
                return Unauthorized("invalid username");
            }

            // hashing
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }
            }

            
            return Ok(GetUserDto(user));
        }

        #endregion

        #region Private methods

        private async Task<bool> UserExists(string username)
        {
            return await _dbContext.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        private UserDto GetUserDto (AppUser user)
        {
            var userDto = new UserDto { Username = user.UserName, Token = _tokenService.CreateToken(user), PhotoUrl = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url };
            return userDto;
        }

        #endregion

    }
}
