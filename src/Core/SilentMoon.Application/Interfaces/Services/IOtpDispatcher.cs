using SilentMoon.Application.Features.Auth.Events;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IOtpDispatcher
    {
        Task SendAsync(string email, string firstName, OtpPurpose purpose);
    }
}