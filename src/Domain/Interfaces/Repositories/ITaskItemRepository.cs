using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface ITaskItemRepository
{
    public Task<Response<ICollection<TaskItem>>> GetTaskItemsByProjectAsync(int projectId);
    public Task<Response<bool>> AddTaskItemAsync(TaskItem taskItem);
    public Task<Response<bool>> AddCommentAsync(Comment comment);
    public Task<Response<bool>> UpdateTaskItemAsync(TaskItem taskItem, int userId);
    public Task<Response<bool>> DeleteTaskItemAsync(int taskId);
}