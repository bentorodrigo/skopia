using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface IProjectService
{
    public Task<Response<ICollection<Project>>> ListAllByUserAsync(int userId);
    public Task<Response<bool>> AddProjectAsync(Project project);
}