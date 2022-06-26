namespace Domain.DatingSite
{
    /// <summary>
    /// class for managing many to many entity relationship for likes
    /// </summary>
    public class UserLike
    {
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public AppUser LikedUser { get; set; }
        public int LikedUserId { get; set; }
    }
}
