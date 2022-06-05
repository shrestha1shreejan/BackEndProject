using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DatingSite
{
    [Table("Photos")]
    public class Photo
    {
        public int Id { get; set; }
        public string? Url { get; set; }
        public bool IsMain { get; set; }
        public string? PublicId { get; set; }

        // fully defining AppUser class here so cascading delete works
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
    }
}