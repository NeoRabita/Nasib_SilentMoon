

using System.ComponentModel.DataAnnotations;

namespace SilentMoon.Application.DTOs.Email
{
    public class VerifyEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string OtpCode { get; set; }
    }
}
