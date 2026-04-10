using SnapMyBook.Application.Abstractions;

namespace SnapMyBook.Infrastructure.Persistence;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly SnapMyBookDbContext _dbContext;

    public EfUnitOfWork(SnapMyBookDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
