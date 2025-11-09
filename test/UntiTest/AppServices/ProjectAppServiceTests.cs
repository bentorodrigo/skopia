using Application.AppServices;
using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces.Services;
using FluentAssertions;
using Moq;

namespace UnitTest.AppServices;

public class ProjectAppServiceTests
{
    private readonly Mock<IProjectService> _mockProjectService;
    private readonly ProjectAppService _appService;

    public ProjectAppServiceTests()
    {
        _mockProjectService = new Mock<IProjectService>();
        _appService = new ProjectAppService(_mockProjectService.Object);
    }

    [Fact]
    public async Task AddProjectAsync_ShouldReturnResponseTrue_WhenServiceSucceeds()
    {
        // Arrange
        var dto = new CreateProjectDTO
        {
            Name = "Novo Projeto",
            Description = "Projeto de Teste",
            UserId = 1
        };

        var expectedResponse = new Response<bool>(true);

        _mockProjectService
            .Setup(s => s.AddProjectAsync(It.IsAny<Project>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _appService.AddProjectAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeTrue();

        _mockProjectService.Verify(s =>
            s.AddProjectAsync(It.Is<Project>(p =>
                p.Name == dto.Name &&
                p.Description == dto.Description &&
                p.UserId == dto.UserId)),
            Times.Once);
    }

    [Fact]
    public async Task AddProjectAsync_ShouldReturnResponseFalse_WhenServiceFails()
    {
        // Arrange
        var dto = new CreateProjectDTO
        {
            Name = "Falha",
            Description = "Erro ao salvar",
            UserId = 1
        };

        var expectedResponse = new Response<bool>(false, "Erro ao salvar projeto");

        _mockProjectService
            .Setup(s => s.AddProjectAsync(It.IsAny<Project>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _appService.AddProjectAsync(dto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Erro ao salvar projeto");
    }

    [Fact]
    public async Task ListAllByUserAsync_ShouldReturnMappedDTOs_WhenProjectsExist()
    {
        // Arrange
        var userId = 10;
        var projects = new List<Project>
        {
            new Project { Id = 1, Name = "Projeto 1", Description = "Desc 1", UserId = userId },
            new Project { Id = 2, Name = "Projeto 2", Description = "Desc 2", UserId = userId }
        };

        var serviceResponse = new Response<ICollection<Project>>(projects);

        _mockProjectService
            .Setup(s => s.ListAllByUserAsync(userId))
            .ReturnsAsync(serviceResponse);

        // Act
        var result = await _appService.ListAllByUserAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);

        var dto = result.Data.First();
        dto.Should().BeOfType<ListProjectDTO>();
        dto.UserId.Should().Be(userId);
        dto.Name.Should().Be("Projeto 1");
        dto.Description.Should().Be("Desc 1");

        _mockProjectService.Verify(s => s.ListAllByUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task ListAllByUserAsync_ShouldReturnEmptyList_WhenNoProjectsFound()
    {
        // Arrange
        var userId = 5;
        var emptyResponse = new Response<ICollection<Project>>(new List<Project>());

        _mockProjectService
            .Setup(s => s.ListAllByUserAsync(userId))
            .ReturnsAsync(emptyResponse);

        // Act
        var result = await _appService.ListAllByUserAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();

        _mockProjectService.Verify(s => s.ListAllByUserAsync(userId), Times.Once);
    }
}