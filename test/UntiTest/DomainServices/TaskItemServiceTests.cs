using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces.Repositories;
using Domain.Services;
using FluentAssertions;
using Moq;

namespace UnitTest.DomainServices;

public class TaskItemServiceTests
{
    private readonly Mock<ITaskItemRepository> _repositoryMock;
    private readonly TaskItemService _service;

    public TaskItemServiceTests()
    {
        _repositoryMock = new Mock<ITaskItemRepository>();
        _service = new TaskItemService(_repositoryMock.Object);
    }

    [Fact]
    public async Task AddCommentAsync_ShouldCallRepositoryAndReturnResult()
    {
        // Arrange
        var comment = new Comment { Author = "User", Content = "Teste" };
        var expectedResponse = new Response<bool>(true);
        _repositoryMock.Setup(r => r.AddCommentAsync(comment))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.AddCommentAsync(comment);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        _repositoryMock.Verify(r => r.AddCommentAsync(comment), Times.Once);
    }

    [Fact]
    public async Task AddTaskItemAsync_ShouldCallRepositoryAndReturnResult()
    {
        // Arrange
        var taskItem = new TaskItem(TaskPriority.Alta)
        {
            Title = "Nova Tarefa",
            Description = "Teste"
        };
        var expectedResponse = new Response<bool>(true);
        _repositoryMock.Setup(r => r.AddTaskItemAsync(taskItem))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.AddTaskItemAsync(taskItem);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        _repositoryMock.Verify(r => r.AddTaskItemAsync(taskItem), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskItemAsync_ShouldCallRepositoryAndReturnResult()
    {
        // Arrange
        var expectedResponse = new Response<bool>(true);
        _repositoryMock.Setup(r => r.DeleteTaskItemAsync(1))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.DeleteTaskItemAsync(1);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        _repositoryMock.Verify(r => r.DeleteTaskItemAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetTaskItemsByProjectAsync_ShouldCallRepositoryAndReturnList()
    {
        // Arrange
        var expectedList = new List<TaskItem>
        {
            new TaskItem(TaskPriority.Media) { Id = 1, Title = "Tarefa 1" },
            new TaskItem(TaskPriority.Alta) { Id = 2, Title = "Tarefa 2" }
        };

        var expectedResponse = new Response<ICollection<TaskItem>>(expectedList);
        _repositoryMock.Setup(r => r.GetTaskItemsByProjectAsync(10))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetTaskItemsByProjectAsync(10);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        result.Data.Should().HaveCount(2);
        _repositoryMock.Verify(r => r.GetTaskItemsByProjectAsync(10), Times.Once);
    }

    [Fact]
    public async Task UpdateTaskItemAsync_ShouldCallRepositoryAndReturnResult()
    {
        // Arrange
        var taskItem = new TaskItem { Id = 5, Description = "Atualizada" };
        var expectedResponse = new Response<bool>(true);

        _repositoryMock.Setup(r => r.UpdateTaskItemAsync(taskItem, 99))
                       .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.UpdateTaskItemAsync(taskItem, 99);

        // Assert
        result.Should().BeEquivalentTo(expectedResponse);
        _repositoryMock.Verify(r => r.UpdateTaskItemAsync(taskItem, 99), Times.Once);
    }
}
