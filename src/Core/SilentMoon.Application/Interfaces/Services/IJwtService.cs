

using SilentMoon.Application.DTOs.JWT;
using SilentMoon.Domain.Entities;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IJwtService
    {
        JwtTokenDto GenerateAccessToken(ApplicationUser user);
        RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
