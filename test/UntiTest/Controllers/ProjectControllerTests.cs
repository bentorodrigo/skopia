using API.Controllers;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controllers;

public class ProjectControllerTests
{
    private readonly Mock<IProjectAppService> _mockService;
    private readonly ProjectController _controller;

    public ProjectControllerTests()
    {
        _mockService = new Mock<IProjectAppService>();
        _controller = new ProjectController(_mockService.Object);
    }

    [Fact]
    public async Task ListProjectsByUser_ShouldReturnOk_WhenProjectsExist()
    {
        // Arrange
        int userId = 1;
        var mockProjects = new List<ListProjectDTO>
        {
            new ListProjectDTO { Id = 1, Name = "Projeto A", UserId = userId },
            new ListProjectDTO { Id = 2, Name = "Projeto B", UserId = userId }
        };

        _mockService
            .Setup(s => s.ListAllByUserAsync(userId))
            .ReturnsAsync(new Response<ICollection<ListProjectDTO>>(mockProjects));

        // Act
        var result = await _controller.ListProjectsByUser(userId);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(mockProjects);

        _mockService.Verify(s => s.ListAllByUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task ListProjectsByUser_ShouldReturnNotFound_WhenNoProjectsExist()
    {
        // Arrange
        int userId = 2;
        var emptyProjects = new List<ListProjectDTO>();

        _mockService
            .Setup(s => s.ListAllByUserAsync(userId))
            .ReturnsAsync(new Response<ICollection<ListProjectDTO>>(emptyProjects));

        // Act
        var result = await _controller.ListProjectsByUser(userId);

        // Assert
        var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFoundResult.Value.Should().Be($"Nenhum projeto encontrado para o usuario {userId} informado");

        _mockService.Verify(s => s.ListAllByUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task Post_ShouldReturnCreated_WhenProjectIsValid()
    {
        // Arrange
        var dto = new CreateProjectDTO { Name = "Novo Projeto", UserId = 1 };

        _mockService
            .Setup(s => s.AddProjectAsync(dto))
            .ReturnsAsync(new Response<bool>(true));

        // Act
        var result = await _controller.Post(dto);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(ProjectController.ListProjectsByUser));
        createdResult.Value.Should().BeEquivalentTo(dto);

        _mockService.Verify(s => s.AddProjectAsync(dto), Times.Once);
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var dto = new CreateProjectDTO { Name = "", UserId = 0 };
        _controller.ModelState.AddModelError("Name", "O nome é obrigatório.");

        // Act
        var result = await _controller.Post(dto);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().BeAssignableTo<SerializableError>();

        _mockService.Verify(s => s.AddProjectAsync(It.IsAny<CreateProjectDTO>()), Times.Never);
    }
}