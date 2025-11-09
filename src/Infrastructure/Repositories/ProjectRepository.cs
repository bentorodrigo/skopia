using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppDbContext _dbContext;

    public ProjectRepository(IUnitOfWork unitOfWork,
                             AppDbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
    }

    public async Task<Response<bool>> AddProjectAsync(Project project)
    {
        await _dbContext.Projects.AddAsync(project);
        var result = await _unitOfWork.CommitAsync();
        return new Response<bool>(result);
    }

    public async Task<Response<ICollection<Project>>> ListAllByUserAsync(int userId)
    {
        var projects = await _dbContext.Projects
            .Where(x => x.UserId == userId)
            .ToListAsync();

        return new Response<ICollection<Project>>(projects);
    }
}