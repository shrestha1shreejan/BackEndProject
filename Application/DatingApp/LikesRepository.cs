//using Application.Common.Helpers;
//using Application.Common.Interface;
//using Application.DatingApp.Interface;
//using Domain.Common.ExtensionMethods;
//using Domain.DatingSite;
//using Domain.DatingSite.Dtos;
//using Microsoft.EntityFrameworkCore;

//namespace Application.DatingApp 
//{
//    internal class LikesRepository : ILikesRepository
//    {
//        private readonly IDbContext _context;

//        #region Constructor
//        public LikesRepository(IDbContext context)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(IDbContext));
//        }
//        #endregion      

//        #region Implementation

//        public async Task<UserLike> GetUserLike(int sourceUserId, int likeUserId)
//        {
//            return await _context.Likes.FindAsync(sourceUserId, likeUserId);
//        }

//        public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
//        {
//            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
//            var likes = _context.Likes.AsQueryable();
            
//            /// list of users liked by current user
//            if (likesParams.Predicate == "liked")
//            {
//                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
//                users = likes.Select(like => like.LikedUser);
//            }

//            /// list of users that like current user
//            if (likesParams.Predicate == "likedBy")
//            {
//                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
//                users = likes.Select(like => like.SourceUser);
//            }

//            var likedUsers = users.Select(user => new LikeDto 
//            { 
//                Username = user.UserName,
//                KnownAs = user.KnownAs,
//                Age = user.DateOfBirth.CalculateAge(),
//                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
//                City = user.City,
//                Id = user.Id
//            });

//            return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
//        }

//        public async Task<AppUser> GetUserWithLikes(int userId)
//        {
//            return await _context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(x => x.Id == userId);
//        }


//        #endregion
//    }
//}
