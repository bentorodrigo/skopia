using Application.AppServices;
using Application.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Services;
using FluentAssertions;
using Moq;

namespace UnitTest.AppServices;

public class TaskItemAppServiceTests
{
    private readonly Mock<ITaskItemService> _mockTaskItemService;
    private readonly TaskItemAppService _appService;

    public TaskItemAppServiceTests()
    {
        _mockTaskItemService = new Mock<ITaskItemService>();
        _appService = new TaskItemAppService(_mockTaskItemService.Object);
    }

    [Fact]
    public async Task AddCommentAsync_ShouldReturnSuccess_WhenServiceSucceeds()
    {
        // Arrange
        var dto = new CreateCommentDTO { Author = "João", Content = "Comentário teste", TaskItemId = 1 };
        var expectedResponse = new Response<bool>(true);

        _mockTaskItemService
            .Setup(s => s.AddCommentAsync(It.IsAny<Comment>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _appService.AddCommentAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        _mockTaskItemService.Verify(s => s.AddCommentAsync(It.Is<Comment>(
            c => c.Author == dto.Author &&
                 c.Content == dto.Content &&
                 c.TaskItemId == dto.TaskItemId)), Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_ShouldReturnFailure_WhenServiceFails()
    {
        // Arrange
        var dto = new CreateCommentDTO { Author = "João", Content = "Falha", TaskItemId = 1 };
        var expectedResponse = new Response<bool>(false, "Erro ao adicionar comentário");

        _mockTaskItemService
            .Setup(s => s.AddCommentAsync(It.IsAny<Comment>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _appService.AddCommentAsync(dto);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Erro ao adicionar comentário");
    }

    [Fact]
    public async Task AddTaskItemAsync_ShouldReturnSuccess_WhenServiceSucceeds()
    {
        // Arrange
        var dto = new CreateTaskItemDTO
        {
            Title = "Tarefa Teste",
            Description = "Descrição da tarefa",
            DueDate = DateTime.Today.AddDays(2),
            ProjectId = 1,
            Priority = TaskPriority.Alta
        };

        var expectedResponse = new Response<bool>(true);

        _mockTaskItemService
            .Setup(s => s.AddTaskItemAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _appService.AddTaskItemAsync(dto);

        // Assert
        result.Success.Should().BeTrue();
        _mockTaskItemService.Verify(s => s.AddTaskItemAsync(It.Is<TaskItem>(
            t => t.Title == dto.Title &&
                 t.Description == dto.Description &&
                 t.ProjectId == dto.ProjectId &&
                 t.Priority == dto.Priority)), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskItemAsync_ShouldReturnSuccess_WhenServiceSucceeds()
    {
        // Arrange
        var expectedResponse = new Response<bool>(true);
        _mockTaskItemService
            .Setup(s => s.DeleteTaskItemAsync(1))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _appService.DeleteTaskItemAsync(1);

        // Assert
        result.Success.Should().BeTrue();
        _mockTaskItemService.Verify(s => s.DeleteTaskItemAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskItemAsync_ShouldReturnFailure_WhenServiceFails()
    {
        // Arrange
        var expectedResponse = new Response<bool>(false, "Tarefa não encontrada");
        _mockTaskItemService
            .Setup(s => s.DeleteTaskItemAsync(2))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _appService.DeleteTaskItemAsync(2);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Tarefa não encontrada");
    }

    [Fact]
    public async Task ListAllByProjectAsync_ShouldReturnTasks_WhenServiceSucceeds()
    {
        // Arrange
        var projectId = 1;
        var taskItems = new List<TaskItem>
        {
            new TaskItem(TaskPriority.Media) { Id = 1, Title = "Tarefa 1", Description = "Teste", ProjectId = projectId, Status = Domain.Enums.TaskStatus.Pendente },
            new TaskItem(TaskPriority.Alta) { Id = 2, Title = "Tarefa 2", Description = "Outro", ProjectId = projectId, Status = Domain.Enums.TaskStatus.Concluida }
        };

        var response = new Response<ICollection<TaskItem>>(taskItems);

        _mockTaskItemService
            .Setup(s => s.GetTaskItemsByProjectAsync(projectId))
            .ReturnsAsync(response);

        // Act
        var result = await _appService.ListAllByProjectAsync(projectId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.First().Title.Should().Be("Tarefa 1");

        _mockTaskItemService.Verify(s => s.GetTaskItemsByProjectAsync(projectId), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskItemAsync_ShouldReturnSuccess_WhenServiceSucceeds()
    {
        // Arrange
        var dto = new UpdateTaskItemDTO { Description = "Atualizada", Status = Domain.Enums.TaskStatus.EmAndamento, UserId = 1 };
        var expectedResponse = new Response<bool>(true);

        _mockTaskItemService
            .Setup(s => s.UpdateTaskItemAsync(It.IsAny<TaskItem>(), dto.UserId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _appService.UpdateTaskItemAsync(10, dto);

        // Assert
        result.Success.Should().BeTrue();
        _mockTaskItemService.Verify(s => s.UpdateTaskItemAsync(It.Is<TaskItem>(
            t => t.Id == 10 && t.Description == "Atualizada" && t.Status == Domain.Enums.TaskStatus.EmAndamento), dto.UserId), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskItemAsync_ShouldReturnFailure_WhenServiceFails()
    {
        // Arrange
        var dto = new UpdateTaskItemDTO { Description = "Falha", Status = Domain.Enums.TaskStatus.Pendente, UserId = 1 };
        var expectedResponse = new Response<bool>(false, "Tarefa não encontrada");

        _mockTaskItemService
            .Setup(s => s.UpdateTaskItemAsync(It.IsAny<TaskItem>(), dto.UserId))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _appService.UpdateTaskItemAsync(99, dto);

        // Assert
        result.Success.Should().BeFalse();
        result.ErrorMessage.Should().Be("Tarefa não encontrada");
    }
}