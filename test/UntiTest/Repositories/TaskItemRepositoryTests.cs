using Domain.Entities;
using Domain.Interfaces.Repositories;
using FluentAssertions;
using Infrastructure.Db;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTest.Repositories;

public class TaskItemRepositoryTests
{
    private static DbContextOptions<AppDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TaskItemRepoTest_{Guid.NewGuid()}")
            .Options;
    }

    private static AppDbContext CreateContext()
    {
        var options = CreateInMemoryOptions();
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static TaskItemRepository CreateRepository(AppDbContext context, Mock<IUnitOfWork> uowMock)
    {
        var loggerMock = new Mock<ILogger<TaskItemRepository>>();
        return new TaskItemRepository(uowMock.Object, context, loggerMock.Object);
    }

    [Fact]
    public async Task AddTaskItemAsync_ShouldFail_WhenProjectNotFound()
    {
        // Arrange
        var context = CreateContext();
        var uowMock = new Mock<IUnitOfWork>();
        var repository = CreateRepository(context, uowMock);

        var taskItem = new TaskItem(Domain.Enums.TaskPriority.Alta) { ProjectId = 999 };

        // Act
        var response = await repository.AddTaskItemAsync(taskItem);

        // Assert
        response.Success.Should().BeFalse();
        response.ErrorMessage.Should().Contain("projeto");
    }

    [Fact]
    public async Task AddTaskItemAsync_ShouldFail_WhenProjectHas20Tasks()
    {
        // Arrange
        var context = CreateContext();
        var uowMock = new Mock<IUnitOfWork>();
        var project = new Project { Id = 1, UserId = 10 };

        for (int i = 0; i < 20; i++)
            project.TaskItems.Add(new TaskItem(Domain.Enums.TaskPriority.Alta) { Id = i + 1 });

        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var repository = CreateRepository(context, uowMock);
        var newTask = new TaskItem(Domain.Enums.TaskPriority.Baixa) { ProjectId = 1 };

        // Act
        var response = await repository.AddTaskItemAsync(newTask);

        // Assert
        response.Success.Should().BeFalse();
        response.ErrorMessage.Should().Contain("limite máximo");
    }

    [Fact]
    public async Task AddCommentAsync_ShouldFail_WhenTaskNotFound()
    {
        // Arrange
        var context = CreateContext();
        var uowMock = new Mock<IUnitOfWork>();
        var repository = CreateRepository(context, uowMock);

        var comment = new Comment { Author = "João", Content = "Comentário", TaskItemId = 99 };

        // Act
        var response = await repository.AddCommentAsync(comment);

        // Assert
        response.Success.Should().BeFalse();
        response.ErrorMessage.Should().Contain("tarefa");
    }

    [Fact]
    public async Task DeleteTaskItemAsync_ShouldFail_WhenNotFound()
    {
        // Arrange
        var context = CreateContext();
        var uowMock = new Mock<IUnitOfWork>();
        var repository = CreateRepository(context, uowMock);

        // Act
        var response = await repository.DeleteTaskItemAsync(123);

        // Assert
        response.Success.Should().BeFalse();
        response.ErrorMessage.Should().Contain("inexistente");
    }

    [Fact]
    public async Task UpdateTaskItemAsync_ShouldFail_WhenTaskNotFound()
    {
        // Arrange
        var context = CreateContext();
        var uowMock = new Mock<IUnitOfWork>();
        var repository = CreateRepository(context, uowMock);

        var task = new TaskItem { Id = 123, Description = "Teste" };

        // Act
        var response = await repository.UpdateTaskItemAsync(task, 1);

        // Assert
        response.Success.Should().BeFalse();
        response.ErrorMessage.Should().Contain("não encontrada");
    }

    [Fact]
    public async Task UpdateTaskItemAsync_ShouldFail_WhenUserNotFound()
    {
        // Arrange
        var context = CreateContext();
        var uowMock = new Mock<IUnitOfWork>();

        var task = new TaskItem(Domain.Enums.TaskPriority.Baixa)
        {
            Id = 1,
            Description = "Teste"
        };

        context.TaskItems.Add(task);
        await context.SaveChangesAsync();

        var repository = CreateRepository(context, uowMock);

        var updated = new TaskItem { Id = 1, Description = "Alterada" };

        // Act
        var response = await repository.UpdateTaskItemAsync(updated, 999);

        // Assert
        response.Success.Should().BeFalse();
        response.ErrorMessage.Should().Contain("usuário");
    }
}
