using System;
using System.Collections.Generic;
using SilentMoon.Application.DTOs.JWT;

namespace SilentMoon.Application.DTOs.Account
{
    public class AuthenticationResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public JwtTokenDto AccessToken { get; set; }
        public RefreshTokenDto RefreshToken { get; set; }
    }
}