using SilentMoon.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SilentMoon.Application.Interfaces.Repositories
{
    public interface IReminderRepository : IGenericRepository<Reminder>
    {
        Task<List<Reminder>> GetUserRemindersAsync(int userId, CancellationToken ct = default);
    }
}
