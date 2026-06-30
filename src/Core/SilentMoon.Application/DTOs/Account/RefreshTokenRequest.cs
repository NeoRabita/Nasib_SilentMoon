
using System.ComponentModel.DataAnnotations;

namespace SilentMoon.Application.DTOs.Account
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
