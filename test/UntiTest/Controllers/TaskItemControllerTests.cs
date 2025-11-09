using API.Controllers;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTest.Controllers;

public class TaskItemControllerTests
{
    private readonly Mock<ITaskItemAppService> _mockService;
    private readonly TaskItemController _controller;

    public TaskItemControllerTests()
    {
        _mockService = new Mock<ITaskItemAppService>();
        _controller = new TaskItemController(_mockService.Object);
    }


    [Fact]
    public async Task ListTaskItemsByProject_ShouldReturnOk_WhenTasksExist()
    {
        // Arrange
        int projectId = 1;
        var tasks = new List<ListTaskItemDTO>
        {
            new ListTaskItemDTO { Id = 1, Title = "Tarefa A", ProjectId = projectId },
            new ListTaskItemDTO { Id = 2, Title = "Tarefa B", ProjectId = projectId }
        };
        var response = new Response<ICollection<ListTaskItemDTO>>(tasks);

        _mockService.Setup(s => s.ListAllByProjectAsync(projectId)).ReturnsAsync(response);

        // Act
        var result = await _controller.ListTaskItemsByProject(projectId);

        // Assert
        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeEquivalentTo(tasks);
    }

    [Fact]
    public async Task ListTaskItemsByProject_ShouldReturnNotFound_WhenNoTasksExist()
    {
        // Arrange
        int projectId = 1;
        var response = new Response<ICollection<ListTaskItemDTO>>(new List<ListTaskItemDTO>());

        _mockService.Setup(s => s.ListAllByProjectAsync(projectId)).ReturnsAsync(response);

        // Act
        var result = await _controller.ListTaskItemsByProject(projectId);

        // Assert
        var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.Value.Should().Be($"Nenhuma tarefa encontrada para o projeto {projectId} informado.");
    }

    [Fact]
    public async Task ListTaskItemsByProject_ShouldReturnBadRequest_WhenProjectIdIsInvalid()
    {
        // Act
        var result = await _controller.ListTaskItemsByProject(0);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("O ID do projeto deve ser maior que zero.");
    }


    [Fact]
    public async Task Post_ShouldReturnCreated_WhenTaskIsValid()
    {
        // Arrange
        var dto = new CreateTaskItemDTO { ProjectId = 1, Title = "Nova Tarefa", Description = "Unit Test", DueDate = DateTime.Now, Priority = TaskPriority.Media };
        var response = new Response<bool>(true);

        _mockService.Setup(s => s.AddTaskItemAsync(dto)).ReturnsAsync(response);

        // Act
        var result = await _controller.Post(dto);

        // Assert
        var created = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        created.ActionName.Should().Be(nameof(TaskItemController.ListTaskItemsByProject));
    }

    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var dto = new CreateTaskItemDTO { ProjectId = 1 };
        _controller.ModelState.AddModelError("Title", "Campo obrigatório");

        // Act
        var result = await _controller.Post(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mockService.Verify(s => s.AddTaskItemAsync(It.IsAny<CreateTaskItemDTO>()), Times.Never);
    }


    [Fact]
    public async Task PostComment_ShouldReturnCreated_WhenCommentIsValid()
    {
        // Arrange
        var dto = new CreateCommentDTO { TaskItemId = 1, Content = "Comentário" };
        var response = new Response<bool>(true);

        _mockService.Setup(s => s.AddCommentAsync(dto)).ReturnsAsync(response);

        // Act
        var result = await _controller.PostComment(dto);

        // Assert
        result.Should().BeOfType<CreatedResult>();
    }

    [Fact]
    public async Task PostComment_ShouldReturnNotFound_WhenServiceFails()
    {
        // Arrange
        var dto = new CreateCommentDTO { TaskItemId = 99, Content = "Comentário" };
        var response = new Response<bool>(false, "Tarefa não encontrada");

        _mockService.Setup(s => s.AddCommentAsync(dto)).ReturnsAsync(response);

        // Act
        var result = await _controller.PostComment(dto);

        // Assert
        var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.Value.Should().Be("Tarefa não encontrada");
    }

    [Fact]
    public async Task PostComment_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var dto = new CreateCommentDTO();
        _controller.ModelState.AddModelError("Content", "Obrigatório");

        // Act
        var result = await _controller.PostComment(dto);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mockService.Verify(s => s.AddCommentAsync(It.IsAny<CreateCommentDTO>()), Times.Never);
    }


    [Fact]
    public async Task Put_ShouldReturnOk_WhenUpdateSucceeds()
    {
        // Arrange
        int id = 1;
        var dto = new UpdateTaskItemDTO { Description = "Unit Test", Status = Domain.Enums.TaskStatus.EmAndamento, UserId = 1 };
        var response = new Response<bool>(true);

        _mockService.Setup(s => s.UpdateTaskItemAsync(id, dto)).ReturnsAsync(response);

        // Act
        var result = await _controller.Put(id, dto);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenUpdateFails()
    {
        // Arrange
        int id = 1;
        var dto = new UpdateTaskItemDTO { Description = "Unit Test", Status = Domain.Enums.TaskStatus.EmAndamento, UserId = 1 };
        var response = new Response<bool>(false, "Tarefa não encontrada");

        _mockService.Setup(s => s.UpdateTaskItemAsync(id, dto)).ReturnsAsync(response);

        // Act
        var result = await _controller.Put(id, dto);

        // Assert
        var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.Value.Should().Be("Tarefa não encontrada");
    }

    [Fact]
    public async Task Put_ShouldReturnBadRequest_WhenIdIsInvalid()
    {
        // Act
        var result = await _controller.Put(0, new UpdateTaskItemDTO());

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("O ID da tarefa deve ser maior que zero.");
    }


    [Fact]
    public async Task Delete_ShouldReturnOk_WhenTaskDeletedSuccessfully()
    {
        // Arrange
        int id = 1;
        var response = new Response<bool>(true);

        _mockService.Setup(s => s.DeleteTaskItemAsync(id)).ReturnsAsync(response);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenTaskDoesNotExist()
    {
        // Arrange
        int id = 5;
        var response = new Response<bool>(false, "Tarefa não encontrada");

        _mockService.Setup(s => s.DeleteTaskItemAsync(id)).ReturnsAsync(response);

        // Act
        var result = await _controller.Delete(id);

        // Assert
        var notFound = result.Should().BeOfType<NotFoundObjectResult>().Subject;
        notFound.Value.Should().Be("Tarefa não encontrada");
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest_WhenIdIsInvalid()
    {
        // Act
        var result = await _controller.Delete(0);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.Value.Should().Be("O ID da tarefa deve ser maior que zero.");
    }
}