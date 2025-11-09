using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Db;

namespace Infrastructure.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _dbContext;

    public ReportRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Response<object>> GetReportsByUser(int userId)
    {
        var report = _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => new {
                UsuarioId = u.Id,
                EmailUsuario = u.Email,
                NomeUsuario = u.Name,
                TarefasConcluidas = u.Projects
                    .SelectMany(p => p.TaskItems)
                    .Where(t => t.Status == Domain.Enums.TaskStatus.Concluida && t.DueDate > DateTime.Now.AddDays(-30))
                    .Count(),
                MediaTarefasConcluidas = u.Projects
                    .SelectMany(p => p.TaskItems)
                    .Where(t => t.Status == Domain.Enums.TaskStatus.Concluida && t.DueDate > DateTime.Now.AddDays(-30))
                    .Count() / (double) u.Projects.Count
            })
            .ToList();

        return new Response<object>(report);
    }
}