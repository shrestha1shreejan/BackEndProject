using Application.DatingApp.Interface;
using AutoMapper;
using Domain.Common.ExtensionMethods;
using Domain.DatingSite;
using Domain.DatingSite.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DataingAppApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        #region Constructor

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _photoService = photoService;
            _mapper = mapper;
        }

        #endregion

        #region methods

        [HttpGet]        
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetMembersAsync();           
            return Ok(users);
        }
        
        /// <summary>
        /// method for getting users
        /// Specifying the route name for using as Created at route params
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        [HttpGet("{username}",Name = "GetUser")]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await _userRepository.GetMemberAsync(username);            
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser (MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            _mapper.Map(memberUpdateDto, user);
            _userRepository.Update(user);
            if (await _userRepository.SaveAllAsync())
            {
                return NoContent();
            }
            return BadRequest("Failed to update user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByUsernameAsync(username);

            var result = await _photoService.AddPhotoAsync(file); // add photo to cloudinary account

            if (result.Error != null)
            {
                return BadRequest(result.Error.Message);
            }

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };

            // making the photot the main photo if incase its first uploaded photo
            if (user?.Photos?.Count == 0)
            {
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                // return _mapper.Map<PhotoDto>(photo);
                // return CreatedAtRoute("GetUser", _mapper.Map<PhotoDto>(photo));
                return CreatedAtRoute("GetUser", new {username = user.UserName} ,_mapper.Map<PhotoDto>(photo));
            }

            return BadRequest("Problem adding photo");

        }
        #endregion

    }
}
