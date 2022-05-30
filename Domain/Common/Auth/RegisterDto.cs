using System.ComponentModel.DataAnnotations;

namespace Domain.Common.Auth
{
    public class RegisterDto
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
