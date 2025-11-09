using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<TaskItemRepository> _logger;

    public TaskItemRepository(IUnitOfWork unitOfWork,
                              AppDbContext dbContext,
                              ILogger<TaskItemRepository> logger)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Response<bool>> AddTaskItemAsync(TaskItem taskItem)
    {
        var project = await _dbContext.Projects
            .Include(p => p.TaskItems)
            .FirstOrDefaultAsync(p => p.Id == taskItem.ProjectId);

        if (project == null)
        {
            var errorMessage = "Não foi possível encontrar o projeto na base de dados";
            _logger.LogInformation(errorMessage);
            return new Response<bool>(false, errorMessage);
        }

        var resultInsertTask = project.AddTaskItem(taskItem);
        if (!resultInsertTask)
        {
            var errorMessage = "O projeto atingiu o limite máximo de 20 tarefas";
            _logger.LogInformation(errorMessage);
            return new Response<bool>(false, errorMessage);
        }

        _dbContext.TaskItems.Add(taskItem);
        var history = new TaskHistory
        {
            TaskItemId = taskItem.Id,
            ModifiedAt = DateTime.Now,
            Modification = $"Tarefa criada com prioridade {taskItem.Priority} e status {taskItem.Status}.",
            ModifiedBy = project.UserId.ToString()
        };

        _dbContext.Histories.Add(history);
        return new Response<bool>(await _unitOfWork.CommitAsync());
    }

    public async Task<Response<bool>> AddCommentAsync(Comment comment)
    {
        var taskItem = await _dbContext.TaskItems
            .FirstOrDefaultAsync(p => p.Id == comment.TaskItemId);

        if (taskItem == null)
        {
            var errorMessage = "Não foi possível encontrar a tarefa na base de dados para a inserção do comentário";
            _logger.LogInformation(errorMessage);
            return new Response<bool>(false, errorMessage);
        }

        comment.TaskItem = taskItem;
        _dbContext.Comments.Add(comment);

        return new Response<bool>(await _unitOfWork.CommitAsync());
    }

    public async Task<Response<bool>> DeleteTaskItemAsync(int taskId)
    {
        var taskItem = await _dbContext.TaskItems
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == taskId);

        if (taskItem == null)
        {
            var errorMessage = "Tarefa inexistente para exclusão";
            _logger.LogInformation(errorMessage);
            return new Response<bool>(false, errorMessage);
        }

        _dbContext.TaskItems.Remove(taskItem);
        return new Response<bool>(await _unitOfWork.CommitAsync());
    }

    public async Task<Response<ICollection<TaskItem>>> GetTaskItemsByProjectAsync(int projectId)
    {
        var taskItems = await _dbContext.TaskItems
            .AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .ToListAsync();

        return new Response<ICollection<TaskItem>>(taskItems);
    }

    public async Task<Response<bool>> UpdateTaskItemAsync(TaskItem taskItem, int userId)
    {
        var taskItemDb = await _dbContext.TaskItems
            .AsTracking()
            .FirstOrDefaultAsync(x => x.Id == taskItem.Id);

        if (taskItemDb == null)
        {
            var errorMessage = "Tarefa não encontrada no banco de dados, não é possível atualizar";
            _logger.LogInformation(errorMessage);
            return new Response<bool>(false, errorMessage);
        }

        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user == null)
        {
            var errorMessage = "Não foi possível encontrar o usuário no sistema para realizar a atualização";
            _logger.LogInformation(errorMessage);
            return new Response<bool>(false, errorMessage);
        }

        if (taskItem.Status != taskItemDb.Status)
        {
            taskItemDb.Historic.Add(new TaskHistory
            {
                Modification = $"O Status da tarefa foi alterado de {taskItemDb.Status} para {taskItem.Status}",
                ModifiedAt = DateTime.Now,
                ModifiedBy = user.Name
            });

            taskItemDb.Status = taskItem.Status;
        }

        if (taskItem.Description != taskItemDb.Description)
        {
            taskItemDb.Historic.Add(new TaskHistory
            {
                Modification = $"A descrição da tarefa foi alterado de {taskItemDb.Description} para {taskItem.Description}",
                ModifiedAt = DateTime.Now,
                ModifiedBy = user.Name
            });

            taskItemDb.Description = taskItem.Description;
        }

        return new Response<bool>(await _unitOfWork.CommitAsync());
    }
}
