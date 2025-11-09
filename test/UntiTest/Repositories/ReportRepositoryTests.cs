using Domain.Entities;
using FluentAssertions;
using Infrastructure.Db;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UnitTest.Repositories;

public class ReportRepositoryTests
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
    public async Task GetReportsByUser_ShouldReturnCorrectReportData()
    {
        // Arrange
        var context = CreateContext();

        var user = new User
        {
            Id = 10,
            Name = "João da Silva",
            Email = "joao@example.com",
            Function = Domain.Enums.UserFunction.Gerente,
            Projects = new List<Project>
        {
            new Project
            {
                Id = 1,
                Name = "Projeto A",
                TaskItems = new List<TaskItem>
                {
                    new TaskItem(Domain.Enums.TaskPriority.Alta)
                    {
                        Id = 1,
                        Title = "Tarefa 1",
                        Status = Domain.Enums.TaskStatus.Concluida,
                        DueDate = DateTime.Now.AddDays(-5)
                    },
                    new TaskItem(Domain.Enums.TaskPriority.Media)
                    {
                        Id = 2,
                        Title = "Tarefa 2",
                        Status = Domain.Enums.TaskStatus.EmAndamento,
                        DueDate = DateTime.Now.AddDays(2)
                    }
                }
            },
            new Project
            {
                Id = 2,
                Name = "Projeto B",
                TaskItems = new List<TaskItem>
                {
                    new TaskItem(Domain.Enums.TaskPriority.Baixa)
                    {
                        Id = 3,
                        Title = "Tarefa 3",
                        Status = Domain.Enums.TaskStatus.Concluida,
                        DueDate = DateTime.Now.AddDays(-10)
                    }
                }
            }
        }
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new ReportRepository(context);

        // Act
        var response = await repository.GetReportsByUser(user.Id);

        // Assert
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();

        var reportList = (response.Data as IEnumerable<object>)!.ToList();
        reportList.Should().HaveCount(1);

        var report = reportList.First();
        var reportDict = report.GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(report));

        reportDict["UsuarioId"].Should().Be(10);
        reportDict["NomeUsuario"].Should().Be("João da Silva");
        reportDict["EmailUsuario"].Should().Be("joao@example.com");
        reportDict["TarefasConcluidas"].Should().Be(2);
    }

    [Fact]
    public async Task GetReportsByUser_ShouldReturnEmptyList_WhenUserDoesNotExist()
    {
        // Arrange
        var context = CreateContext();
        var repository = new ReportRepository(context);

        // Act
        var response = await repository.GetReportsByUser(999);

        // Assert
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();

        var reportList = (response.Data as IEnumerable<object>)!.ToList();
        reportList.Should().BeEmpty();
    }

    [Fact]
    public async Task GetReportsByUser_ShouldReturnZeroCompletedTasks_WhenNoRecentCompletions()
    {
        // Arrange
        var context = CreateContext();

        var user = new User
        {
            Id = 20,
            Name = "Maria Oliveira",
            Email = "maria@example.com",
            Function = Domain.Enums.UserFunction.Gerente,
            Projects = new List<Project>
        {
            new Project
            {
                Id = 5,
                Name = "Projeto X",
                TaskItems = new List<TaskItem>
                {
                    new TaskItem(Domain.Enums.TaskPriority.Alta)
                    {
                        Id = 50,
                        Title = "Antiga",
                        Status = Domain.Enums.TaskStatus.Concluida,
                        DueDate = DateTime.Now.AddDays(-40) // Fora do intervalo de 30 dias
                    }
                }
            }
        }
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var repository = new ReportRepository(context);

        // Act
        var response = await repository.GetReportsByUser(user.Id);

        // Assert
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();

        var reportList = (response.Data as IEnumerable<object>)!.ToList();
        reportList.Should().HaveCount(1);

        var report = reportList.First();
        var reportDict = report.GetType()
            .GetProperties()
            .ToDictionary(p => p.Name, p => p.GetValue(report));

        reportDict["TarefasConcluidas"].Should().Be(0);
        reportDict["MediaTarefasConcluidas"].Should().Be(0.0);
    }
}
