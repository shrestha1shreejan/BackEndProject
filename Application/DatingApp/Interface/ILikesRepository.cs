using Application.Common.Helpers;
using Domain.DatingSite;
using Domain.DatingSite.Dtos;

namespace Application.DatingApp.Interface
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likeUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        /// <summary>
        /// predicate defines what are we looking for (users that have been liked or liked by
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}
