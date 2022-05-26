using Application.Common.Interface;
using Domain.DatingSite;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DataingAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDbContext _dbContext;

        #region Constructor

        public UsersController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region methods

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _dbContext.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            return Ok(user);
        }

        #endregion

    }
}
