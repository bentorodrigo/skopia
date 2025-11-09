using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces;

public interface ITaskItemAppService
{
    public Task<Response<ICollection<ListTaskItemDTO>>> ListAllByProjectAsync(int projectId);
    public Task<Response<bool>> AddTaskItemAsync(CreateTaskItemDTO taskItemDTO);
    public Task<Response<bool>> AddCommentAsync(CreateCommentDTO createCommentDTO);
    public Task<Response<bool>> UpdateTaskItemAsync(int id, UpdateTaskItemDTO taskItemDTO);
    public Task<Response<bool>> DeleteTaskItemAsync(int id);
}