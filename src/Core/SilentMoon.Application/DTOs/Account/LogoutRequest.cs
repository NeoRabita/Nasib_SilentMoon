

using System.ComponentModel.DataAnnotations;

namespace SilentMoon.Application.DTOs.Account
{
    public class LogoutRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
