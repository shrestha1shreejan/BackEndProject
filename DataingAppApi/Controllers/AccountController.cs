using Application.Token;
using AutoMapper;
using Domain.Common.Auth;
using Domain.DatingSite;
using Domain.DatingSite.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataingAppApi.Controllers
{
    public class AccountController : BaseApiController
    {
        // private readonly DataContext _dbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        #region Constructor

        // public AccountController(DataContext dbContext, ITokenService tokenService, IMapper mapper)
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager
            , ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            user.UserName = registerDto.Username.ToLower();
            // hashing
            // using var hmac = new HMACSHA512();
            // Commenting as we are using asp net core identity for this now
            //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            //user.PasswordSalt = hmac.Key;

            //_dbContext.Users.Add(user);
            //await _dbContext.SaveChangesAsync();
            var result = await _userManager.CreateAsync(user, registerDto.Password); // creates user and saves it to DB          

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // adding all new user to Member role
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");

            if (!roleResult.Succeeded)  return BadRequest(roleResult.Errors);

            return Ok(GetUserDtoAsync(user).Result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppUser>> Login (LoginDto loginDto)
        {
            // var user = await _dbContext.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName == loginDto.Username);            
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            if (user == null)
            {
                return Unauthorized("invalid username");
            }

            // hashing
            // using var hmac = new HMACSHA512(user.PasswordSalt);
            // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
            // for (int i = 0; i < computedHash.Length; i++)
            // {
            //    if (computedHash[i] != user.PasswordHash[i])
            //    {
            //        return Unauthorized("Invalid password");
            //    }
            // }


            /// using signInManger to signing in
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized();
            }

            return Ok(GetUserDtoAsync(user).Result);
        }

        #endregion

        #region Private methods

        private async Task<bool> UserExists(string username)
        {
            // return await _dbContext.Users.AnyAsync(x => x.UserName == username.ToLower());
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        private async Task<UserDto> GetUserDtoAsync (AppUser user)
        {
            var userDto = new UserDto { 
                Username = user.UserName, 
                Token = await _tokenService.CreateToken(user), 
                KnownAs = user?.KnownAs,
                PhotoUrl = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                Gender = user?.Gender
            };
            return userDto;
        }

        #endregion

    }
}
