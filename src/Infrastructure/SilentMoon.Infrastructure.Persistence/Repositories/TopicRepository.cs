using Microsoft.EntityFrameworkCore;
using SilentMoon.Application.Interfaces.Repositories;
using SilentMoon.Domain.Entities;
using SilentMoon.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Infrastructure.Persistence.Repositories
{
    public class TopicRepository : GenericRepository<Topic>, ITopicRepository
    {
        public TopicRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Topic>> GetActiveTopicsAsync(CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(t => t.IsActive)
                .OrderBy(t => t.SortOrder)
                .ToListAsync(ct);
        }

        public async Task<List<Topic>> GetUserTopicsAsync(int userId, CancellationToken ct = default)
        {
            return await _dbContext.Set<UserTopic>()
                .AsNoTracking()
                .Where(ut => ut.UserId == userId && ut.Topic.IsActive)
                .OrderBy(ut => ut.Topic.SortOrder)
                .Select(ut => ut.Topic)
                .ToListAsync(ct);
        }

        public async Task<int> CountExistingActiveAsync(List<int> topicIds, CancellationToken ct = default)
        {
            return await _dbSet
                .CountAsync(t => topicIds.Contains(t.Id) && t.IsActive, ct);
        }

        public async Task ReplaceUserTopicsAsync(int userId, List<int> topicIds, CancellationToken ct = default)
        {
            var userTopics = _dbContext.Set<UserTopic>();

        
            var existing = await userTopics
                .Where(ut => ut.UserId == userId)
                .ToListAsync(ct);

            userTopics.RemoveRange(existing);

         
            foreach (var topicId in topicIds)
            {
                await userTopics.AddAsync(new UserTopic
                {
                    UserId = userId,
                    TopicId = topicId
                }, ct);
            }
        }
    }
}

