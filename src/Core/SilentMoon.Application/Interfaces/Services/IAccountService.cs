
using SilentMoon.Application.DTOs.Account;
using SilentMoon.Application.DTOs.Email;
using SilentMoon.Application.DTOs.JWT;
using SilentMoon.Domain.Entities;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task RegisterAsync(RegisterRequest request);
        Task<AuthenticationResponse> LoginAsync(LoginRequest request, string ipAddress);
        Task VerifyEmailAsync(VerifyEmailRequest request);
        Task ResendOtpAsync(ResendOtpRequest request);
        Task<AuthenticationResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task LogoutAsync(LogoutRequest request);
        Task ForgotPasswordAsync(ForgotPasswordRequest request);
        Task ResetPasswordAsync(ResetPasswordRequest request);
    }
}