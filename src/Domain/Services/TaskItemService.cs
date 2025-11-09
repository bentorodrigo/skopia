using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Domain.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _taskItemRepository;

    public TaskItemService(ITaskItemRepository taskItemRepository)
    {
        _taskItemRepository = taskItemRepository;
    }

    public async Task<Response<bool>> AddCommentAsync(Comment comment)
    {
        return await _taskItemRepository.AddCommentAsync(comment);
    }

    public async Task<Response<bool>> AddTaskItemAsync(TaskItem taskItem)
    {
        return await _taskItemRepository.AddTaskItemAsync(taskItem);
    }

    public async Task<Response<bool>> DeleteTaskItemAsync(int taskId)
    {
        return await _taskItemRepository.DeleteTaskItemAsync(taskId);
    }

    public async Task<Response<ICollection<TaskItem>>> GetTaskItemsByProjectAsync(int projectId)
    {
        return await _taskItemRepository.GetTaskItemsByProjectAsync(projectId);
    }

    public async Task<Response<bool>> UpdateTaskItemAsync(TaskItem taskItem, int userId)
    {
        return await _taskItemRepository.UpdateTaskItemAsync(taskItem, userId);
    }
}