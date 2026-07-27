using SilentMoon.Application.DTOs.Account;
using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface IAuthTokenIssuer
    {
        Task<AuthenticationResponse> IssueAsync(ApplicationUser user, CancellationToken ct);
    }
}