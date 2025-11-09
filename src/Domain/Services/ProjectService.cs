using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Domain.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ProjectService> _logger;

    public ProjectService(IProjectRepository projectRepository, 
                          IUserRepository userRepository, 
                          ILogger<ProjectService> logger)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Response<bool>> AddProjectAsync(Project project)
    {
        var user = await _userRepository.GetByIdAsync(project.UserId);

        if (user == null)
        {
            var errorMessage = "Não foi possível encontrar o usuário informado para a criação do projeto no banco de dados";
            _logger.LogInformation(errorMessage);
            return new Response<bool>(false, errorMessage);
        }

        project.User = user.Data;
        project.UserId = user.Data.Id;

        return await _projectRepository.AddProjectAsync(project);
    }

    public async Task<Response<ICollection<Project>>> ListAllByUserAsync(int userId)
    {
        return await _projectRepository.ListAllByUserAsync(userId);
    }
}