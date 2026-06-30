
using System.ComponentModel.DataAnnotations;

namespace SilentMoon.Application.DTOs.Account
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
