using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IProjectRepository
{
    public Task<Response<ICollection<Project>>> ListAllByUserAsync(int userId);
    public Task<Response<bool>> AddProjectAsync(Project project);
}