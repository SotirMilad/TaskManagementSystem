using TaskManagementSystem.DTOs.Common;
using TaskManagementSystem.DTOs.Tasks.Requests;
using TaskManagementSystem.DTOs.Tasks.Responses;

namespace TaskManagementSystem.Services.IServices
{
    public interface ITaskService
    {
        Task<TaskResponse> CreateAsync(int projectId, CreateTaskRequest request);
        Task<PagedResult<TaskResponse>> GetByProjectAsync(int projectId, TaskQueryParameters query);
        Task<PagedResult<TaskResponse>> GetAllAsync(TaskQueryParameters query);
        Task<TaskResponse> GetByIdAsync(int id);
        Task<TaskResponse> UpdateAsync(int id, UpdateTaskRequest request);
        Task DeleteAsync(int id);
    }

}
