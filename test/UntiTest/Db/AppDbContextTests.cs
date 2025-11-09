using Domain.Entities;
using FluentAssertions;
using Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.Db;

public class AppDbContextTests
{
    private DbContextOptions<AppDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
    }

    [Fact(DisplayName = "Deve criar o banco de dados em memória e aplicar o seed de usuários")]
    public async Task Should_CreateDatabaseAndSeedUsers()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var users = await context.Users.ToListAsync();

        // Assert
        users.Should().NotBeEmpty()
            .And.HaveCount(3);

        users.Should().Contain(u => u.Name == "Rodrigo Bento" && u.Function == Domain.Enums.UserFunction.Gerente);
        users.Should().Contain(u => u.Name == "Maria Silva");
    }

    [Fact(DisplayName = "Deve criar projeto vinculado a um usuário e persistir corretamente")]
    public async Task Should_CreateProjectLinkedToUser()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var user = await context.Users.FirstAsync();

        var project = new Project
        {
            Name = "Novo Projeto",
            Description = "Teste de relacionamento",
            UserId = user.Id
        };

        // Act
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        // Assert
        var savedProject = await context.Projects.Include(p => p.User).FirstAsync();
        savedProject.Should().NotBeNull();
        savedProject.User.Should().NotBeNull();
        savedProject.UserId.Should().Be(user.Id);
    }

    [Fact(DisplayName = "Deve criar tarefa vinculada a um projeto e refletir o relacionamento")]
    public async Task Should_CreateTaskItemLinkedToProject()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var user = await context.Users.FirstAsync();
        var project = new Project { Name = "Projeto Tarefas", UserId = user.Id };
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        var taskItem = new TaskItem(Domain.Enums.TaskPriority.Media)
        {
            Title = "Implementar testes",
            Description = "Criar testes unitários para AppDbContext",
            ProjectId = project.Id
        };

        // Act
        context.TaskItems.Add(taskItem);
        await context.SaveChangesAsync();

        // Assert
        var savedTask = await context.TaskItems.Include(t => t.Project).FirstAsync();
        savedTask.Should().NotBeNull();
        savedTask.Project.Should().NotBeNull();
        savedTask.ProjectId.Should().Be(project.Id);
    }

    [Fact(DisplayName = "Deve criar histórico e comentário vinculados à tarefa")]
    public async Task Should_CreateHistoryAndCommentLinkedToTaskItem()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        await using var context = new AppDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var user = await context.Users.FirstAsync();
        var project = new Project { Name = "Projeto 1", UserId = user.Id };
        var taskItem = new TaskItem(Domain.Enums.TaskPriority.Alta)
        {
            Title = "Tarefa X",
            Description = "Teste de histórico",
            Project = project
        };

        var history = new TaskHistory
        {
            TaskItem = taskItem,
            Modification = "Status atualizado"
        };

        var comment = new Comment
        {
            TaskItem = taskItem,
            Author = "Tester",
            Content = "Comentário de teste"
        };

        // Act
        context.AddRange(project, taskItem, history, comment);
        await context.SaveChangesAsync();

        // Assert
        var savedTask = await context.TaskItems
            .Include(t => t.Historic)
            .Include(t => t.Comments)
            .FirstAsync();

        savedTask.Historic.Should().HaveCount(1);
        savedTask.Comments.Should().HaveCount(1);
    }

    [Fact(DisplayName = "Deve configurar corretamente todas as entidades e chaves primárias")]
    public void Should_HaveValidEntityConfiguration()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new AppDbContext(options);
        var model = context.Model;

        // Assert
        model.FindEntityType(typeof(User))!.FindPrimaryKey()!.Properties.Should().ContainSingle(p => p.Name == "Id");
        model.FindEntityType(typeof(Project))!.FindPrimaryKey()!.Properties.Should().ContainSingle(p => p.Name == "Id");
        model.FindEntityType(typeof(TaskItem))!.FindPrimaryKey()!.Properties.Should().ContainSingle(p => p.Name == "Id");
        model.FindEntityType(typeof(TaskHistory))!.FindPrimaryKey()!.Properties.Should().ContainSingle(p => p.Name == "Id");
        model.FindEntityType(typeof(Comment))!.FindPrimaryKey()!.Properties.Should().ContainSingle(p => p.Name == "Id");
    }
}