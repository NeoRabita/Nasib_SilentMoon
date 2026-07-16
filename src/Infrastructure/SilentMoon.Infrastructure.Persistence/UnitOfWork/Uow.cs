using Microsoft.EntityFrameworkCore.Storage;
using SilentMoon.Application.Interfaces.Repositories;
using SilentMoon.Infrastructure.Persistence.Contexts;
using System.Threading.Tasks;
using System.Threading;

public class Uow : IUow
{
    private readonly AppDbContext _context;
    private IDbContextTransaction? _transaction;

    public IPomodoroRepository PomodoroRepository { get; }
    public IUserRepository UserRepository { get; }
    public IRefreshTokenRepository RefreshTokenRepository { get; }
    public ITopicRepository TopicRepository { get; }
    public IReminderRepository ReminderRepository { get; }

    public Uow(
        AppDbContext context,
        IPomodoroRepository pomodoroRepository,
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITopicRepository topicRepository,              
        IReminderRepository reminderRepository)
    {
        _context = context;
        PomodoroRepository = pomodoroRepository;
        UserRepository = userRepository;
        RefreshTokenRepository = refreshTokenRepository;
        TopicRepository = topicRepository;            
        ReminderRepository = reminderRepository;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null) return;
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}