using SilentMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Repositories
{
    public interface ITopicRepository : IGenericRepository<Topic>
    {
        Task<List<Topic>> GetActiveTopicsAsync(CancellationToken ct = default);
        Task<List<Topic>> GetUserTopicsAsync(int userId, CancellationToken ct = default);
        Task<int> CountExistingActiveAsync(List<int> topicIds, CancellationToken ct = default);
        Task ReplaceUserTopicsAsync(int userId, List<int> topicIds, CancellationToken ct = default);
    }
}
