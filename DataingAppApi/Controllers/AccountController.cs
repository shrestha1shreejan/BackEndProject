using Application.Common.Interface;
using Application.Token;
using AutoMapper;
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
        private readonly IMapper _mapper;

        #region Constructor

        public AccountController(IDbContext dbContext, ITokenService tokenService, IMapper mapper)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
            _mapper = mapper;
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

            var user = _mapper.Map<AppUser>(registerDto);
            // hashing
            using var hmac = new HMACSHA512();

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;
 
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
            var userDto = new UserDto { 
                Username = user.UserName, 
                Token = _tokenService.CreateToken(user), 
                KnownAs = user?.KnownAs,
                PhotoUrl = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url 
            };
            return userDto;
        }

        #endregion

    }
}
