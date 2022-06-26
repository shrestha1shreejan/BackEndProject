using Application.Common.Helpers;
using Application.Common.Interface;
using Application.DatingApp.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.DatingSite;
using Domain.DatingSite.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Application.DatingApp
{
    internal sealed class UserRepository : IUserRepository
    {
        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        #region Constructor
        public UserRepository(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        #endregion

        #region Implementation
        public async Task<IEnumerable<AppUser>> GetUserAsync()
        {
            return await _context.Users.Include(p => p.Photos).ToListAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Update(user);
        }

        /// <summary>
        /// optimizing the DB query using AM to get only required properties
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users
                .Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users
                .Include(p => p.Photos)
                .AsQueryable();
            if(userParams.CurrentUsername!= null)
                query = query.Where(u => u.UserName.ToLower() != userParams.CurrentUsername.ToLower());

            query = query.Where(u => u.Gender == userParams.Gender);

            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(v => v.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync
                (query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(), 
                userParams.PageNumber, userParams.PageSize);
                
        }
        #endregion

    }
}
