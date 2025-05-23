// DTOs/LoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace EduSync.DTOs
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}

