

using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IOtpService
    {
        Task<string> GenerateAsync(string email);
        Task<bool> VerifyAsync(string email, string otp);
        Task RemoveAsync(string email);
    }
}
