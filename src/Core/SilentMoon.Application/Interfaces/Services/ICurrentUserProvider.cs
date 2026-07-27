using SilentMoon.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Services
{
    public interface ICurrentUserProvider
    {
        Task<Result<ApplicationUser>> GetCurrentUserAsync(CancellationToken ct);
    }
}