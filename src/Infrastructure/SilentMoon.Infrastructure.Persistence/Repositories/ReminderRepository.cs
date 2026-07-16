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
    public class ReminderRepository : GenericRepository<Reminder>, IReminderRepository
    {
        public ReminderRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Reminder>> GetUserRemindersAsync(int userId, CancellationToken ct = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.Time)
                .ToListAsync(ct);
        }
    }
}
