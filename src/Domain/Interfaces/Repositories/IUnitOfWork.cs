namespace Domain.Interfaces.Repositories;

public interface IUnitOfWork
{
    public Task<bool> CommitAsync();
}