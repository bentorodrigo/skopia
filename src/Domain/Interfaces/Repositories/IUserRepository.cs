using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<Response<User?>> GetByIdAsync(int id);
}