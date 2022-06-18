using System.ComponentModel.DataAnnotations;

namespace Domain.Common.Auth
{
    public class RegisterDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? KnownAs { get; set; }
        [Required]
        public string? Gender { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Country { get; set; }


        [Required]
        [StringLength(10, MinimumLength = 4)]
        public string? Password { get; set; }
    }
}
