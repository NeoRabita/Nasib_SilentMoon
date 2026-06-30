
using System.ComponentModel.DataAnnotations;

namespace SilentMoon.Application.DTOs.Email
{
    public class ResendOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
