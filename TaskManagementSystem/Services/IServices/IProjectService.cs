using TaskManagementSystem.DTOs.Common;
using TaskManagementSystem.DTOs.Projects.Requests;
using TaskManagementSystem.DTOs.Projects.Responses;

namespace TaskManagementSystem.Services.IServices
{
    public interface IProjectService
    {
        Task<ProjectResponse> CreateAsync(CreateProjectRequest request);

        Task<PagedResult<ProjectResponse>> GetAllAsync(int page, int limit);

        Task<ProjectResponse> GetByIdAsync(int id);

        Task<ProjectResponse> UpdateAsync(int id, UpdateProjectRequest request);

        Task DeleteAsync(int id);
    }
}
