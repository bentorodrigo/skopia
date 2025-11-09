using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces.Services;

namespace Application.AppServices;

public class TaskItemAppService : ITaskItemAppService
{
    private readonly ITaskItemService _taskItemService;

    public TaskItemAppService(ITaskItemService taskItemService)
    {
        _taskItemService = taskItemService;
    }

    public async Task<Response<bool>> AddCommentAsync(CreateCommentDTO createCommentDTO)
    {
        var comment = new Comment
        {
            Author = createCommentDTO.Author,
            Content = createCommentDTO.Content,
            TaskItemId = createCommentDTO.TaskItemId
        };
        return await _taskItemService.AddCommentAsync(comment);
    }

    public async Task<Response<bool>> AddTaskItemAsync(CreateTaskItemDTO taskItemDTO)
    {
        var taskItem = new TaskItem(taskItemDTO.Priority)
        {
            DueDate = taskItemDTO.DueDate,
            Description = taskItemDTO.Description,
            Title = taskItemDTO.Title,
            ProjectId = taskItemDTO.ProjectId
        };
        var result = await _taskItemService.AddTaskItemAsync(taskItem);
        return result;
    }

    public async Task<Response<bool>> DeleteTaskItemAsync(int id)
    {
        return await _taskItemService.DeleteTaskItemAsync(id);
    }

    public async Task<Response<ICollection<ListTaskItemDTO>>> ListAllByProjectAsync(int projectId)
    {
        var taskItems = await _taskItemService.GetTaskItemsByProjectAsync(projectId);
        var taskItemsDTO = taskItems.Data
            .Select(taskItem => new ListTaskItemDTO
            {
                Id = taskItem.Id,
                Description = taskItem.Description,
                DueDate = taskItem.DueDate,
                Priority = taskItem.Priority,
                ProjectId = projectId,
                Status = taskItem.Status,
                Title = taskItem.Title
            }).ToList();
        return new Response<ICollection<ListTaskItemDTO>>(taskItemsDTO);
    }

    public async Task<Response<bool>> UpdateTaskItemAsync(int id, UpdateTaskItemDTO taskItemDTO)
    {
        var taskItem = new TaskItem
        {
            Id = id,
            Description = taskItemDTO.Description,
            Status = taskItemDTO.Status
        };
        return await _taskItemService.UpdateTaskItemAsync(taskItem, taskItemDTO.UserId);
    }
}