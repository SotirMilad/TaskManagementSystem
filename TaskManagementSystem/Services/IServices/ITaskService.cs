using TaskManagementSystem.DTOs.Common;
using TaskManagementSystem.DTOs.Tasks.Requests;
using TaskManagementSystem.DTOs.Tasks.Responses;

namespace TaskManagementSystem.Services.IServices
{
    public interface ITaskService
    {
        Task<TaskResponse> CreateAsync(int userId, int projectId, CreateTaskRequest request);
        Task<PagedResult<TaskResponse>> GetByProjectAsync(int userId, int projectId, TaskQueryParameters query);
        Task<PagedResult<TaskResponse>> GetAllAsync(int userId, TaskQueryParameters query);
        Task<TaskResponse> GetByIdAsync(int userId, int id);
        Task<TaskResponse> UpdateAsync(int userId, int id, UpdateTaskRequest request);
        Task DeleteAsync(int userId, int id);
    }

}
