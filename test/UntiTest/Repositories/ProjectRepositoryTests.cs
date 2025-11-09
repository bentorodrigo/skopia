using Domain.Entities;
using Domain.Interfaces.Repositories;
using FluentAssertions;
using Infrastructure.Db;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTest.Repositories;

public class ProjectRepositoryTests
{
    private static DbContextOptions<AppDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
    }

    private static AppDbContext CreateContext()
    {
        var options = CreateInMemoryOptions();
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task AddProjectAsync_ShouldAddProjectAndReturnSuccess()
    {
        // Arrange
        var context = CreateContext();

        var unitOfWork = new UnitOfWork(context);
        var repository = new ProjectRepository(unitOfWork, context);

        var project = new Project
        {
            Name = "Projeto Teste",
            Description = "Projeto de teste unitário",
            UserId = 1
        };

        // Act
        var response = await repository.AddProjectAsync(project);

        // Assert
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Data.Should().BeTrue();

        var savedProject = await context.Projects.FirstOrDefaultAsync();
        savedProject.Should().NotBeNull();
        savedProject!.Name.Should().Be("Projeto Teste");
    }

    [Fact]
    public async Task AddProjectAsync_ShouldReturnFalseWhenCommitFails()
    {
        // Arrange
        var context = CreateContext();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(false);

        var repository = new ProjectRepository(unitOfWorkMock.Object, context);

        var project = new Project { Name = "Projeto com erro", UserId = 1 };

        // Act
        var response = await repository.AddProjectAsync(project);

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().BeFalse();
        response.Success.Should().BeTrue(); // Pois Response<bool> com Data = false não é erro

        unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
    }

    [Fact]
    public async Task ListAllByUserAsync_ShouldReturnOnlyUserProjects()
    {
        // Arrange
        var context = CreateContext();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repository = new ProjectRepository(unitOfWorkMock.Object, context);

        context.Projects.AddRange(new[]
        {
        new Project { Name = "Projeto A", UserId = 1 },
        new Project { Name = "Projeto B", UserId = 1 },
        new Project { Name = "Projeto C", UserId = 2 }
    });
        await context.SaveChangesAsync();

        // Act
        var response = await repository.ListAllByUserAsync(1);

        // Assert
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
        response.Data.Should().OnlyContain(p => p.UserId == 1);
    }

    [Fact]
    public async Task ListAllByUserAsync_ShouldReturnEmptyListWhenUserHasNoProjects()
    {
        // Arrange
        var context = CreateContext();

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var repository = new ProjectRepository(unitOfWorkMock.Object, context);

        // Act
        var response = await repository.ListAllByUserAsync(999);

        // Assert
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Data.Should().BeEmpty();
    }
}
