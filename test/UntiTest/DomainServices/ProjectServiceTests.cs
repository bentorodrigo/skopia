using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.DomainServices;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<ProjectService>> _loggerMock;
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<ProjectService>>();

        _service = new ProjectService(
            _projectRepositoryMock.Object,
            _userRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task AddProjectAsync_ShouldAddProject_WhenUserExists()
    {
        // Arrange
        var user = new User { Id = 1, Name = "Rodrigo" };
        var project = new Project { Name = "Projeto A", UserId = 1 };

        var userResponse = new Response<User>(user);
        var expectedResponse = new Response<bool>(true);

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(project.UserId))
            .ReturnsAsync(userResponse);

        _projectRepositoryMock
            .Setup(x => x.AddProjectAsync(It.IsAny<Project>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.AddProjectAsync(project);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();

        _projectRepositoryMock.Verify(x => x.AddProjectAsync(It.Is<Project>(p => p.UserId == user.Id)), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByIdAsync(project.UserId), Times.Once);
        _loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task AddProjectAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var project = new Project { Name = "Projeto B", UserId = 99 };

        _userRepositoryMock
            .Setup(x => x.GetByIdAsync(project.UserId))
            .ReturnsAsync((Response<User>)null);

        // Act
        var result = await _service.AddProjectAsync(project);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Não foi possível encontrar o usuário");

        _projectRepositoryMock.Verify(x => x.AddProjectAsync(It.IsAny<Project>()), Times.Never);
    }

    [Fact]
    public async Task ListAllByUserAsync_ShouldReturnProjects_WhenTheyExist()
    {
        // Arrange
        var projects = new List<Project>
    {
        new Project { Id = 1, Name = "Projeto A" },
        new Project { Id = 2, Name = "Projeto B" }
    };

        var expectedResponse = new Response<ICollection<Project>>(projects);

        _projectRepositoryMock
            .Setup(x => x.ListAllByUserAsync(1))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.ListAllByUserAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().Contain(p => p.Name == "Projeto A");

        _projectRepositoryMock.Verify(x => x.ListAllByUserAsync(1), Times.Once);
    }
}
