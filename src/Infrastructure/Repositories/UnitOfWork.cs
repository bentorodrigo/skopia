using Domain.Interfaces.Repositories;
using Infrastructure.Db;

namespace Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CommitAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}