using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces.Services;

namespace Application.AppServices;

public class ProjectAppService : IProjectAppService
{
    private readonly IProjectService _projectService;

    public ProjectAppService(IProjectService projectService)
    {
        _projectService = projectService;
    }

    public async Task<Response<bool>> AddProjectAsync(CreateProjectDTO projectDTO)
    {         
        var project = new Project
        {
            Name = projectDTO.Name,
            Description = projectDTO.Description,
            UserId = projectDTO.UserId
        };

        var result = await _projectService.AddProjectAsync(project);
        return result;
    }

    public async Task<Response<ICollection<ListProjectDTO>>> ListAllByUserAsync(int userId)
    {
        var projects = await _projectService.ListAllByUserAsync(userId);
        var projectsDTO = projects.Data
            .Select(project => new ListProjectDTO { Id = project.Id, UserId = userId, Description = project.Description, Name = project.Name })
            .ToList();
        return new Response<ICollection<ListProjectDTO>>(projectsDTO);
    }
}