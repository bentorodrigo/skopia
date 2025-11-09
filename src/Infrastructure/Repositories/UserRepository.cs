using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Response<User?>> GetByIdAsync(int id)
    {
        return new Response<User?>(await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == id));
    }
}