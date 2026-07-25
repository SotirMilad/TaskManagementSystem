using TaskManagementSystem.DTOs.Common;
using TaskManagementSystem.DTOs.Projects.Requests;
using TaskManagementSystem.DTOs.Projects.Responses;

namespace TaskManagementSystem.Services.IServices
{
    public interface IProjectService
    {
        Task<ProjectResponse> CreateAsync(int userId, CreateProjectRequest request);
        Task<PagedResult<ProjectResponse>> GetAllAsync(int userId, int page, int limit);
        Task<ProjectResponse> GetByIdAsync(int userId, int id);
        Task<ProjectResponse> UpdateAsync(int userId, int id, UpdateProjectRequest request);
        Task DeleteAsync(int userId, int id);
    }
}
