using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface IProjectAppService
{
    public Task<Response<ICollection<ListProjectDTO>>> ListAllByUserAsync(int userId);
    public Task<Response<bool>> AddProjectAsync(CreateProjectDTO projectDTO);
}