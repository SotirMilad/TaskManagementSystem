using TaskManagementSystem.DTOs.Common;
using TaskManagementSystem.DTOs.Projects.Requests;
using TaskManagementSystem.DTOs.Projects.Responses;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services.IServices;
using TaskManagementSystem.Context;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Services.ImplementationServices
{
    public class ProjectService : IProjectService
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(ApplicationDBContext context, ILogger<ProjectService> logger)
        {
            _context = context;
            _logger = logger;    
        }

        public async Task<ProjectResponse> CreateAsync(int userId, CreateProjectRequest request)
        {

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            System.Diagnostics.Debug.WriteLine($"DEBUG: userExists in DB (via app's own context) = {userExists}, userId = {userId}");


            // validate project name
            if (string.IsNullOrWhiteSpace(request.Name))
                throw ApiException.BadRequest("Project name is required.");

            // check duplicate project name
            var nameTaken = await _context.Projects.AnyAsync(p => p.UserId == userId && p.Name == request.Name);
            if (nameTaken)
                throw ApiException.Conflict($"A project named '{request.Name}' already exists.");

            var now = DateTime.UtcNow;

            // create the projectt
            var project = new Project
            {
                UserId = userId,
                Name = request.Name.Trim(),
                Description = request.Description,
                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return ToResponse(project);
        }

        public async Task<PagedResult<ProjectResponse>> GetAllAsync(int userId, int page, int limit)
        {
            // validate pagination
            page = Math.Max(page, 1);
            limit = Math.Clamp(limit, 1, 200);

            var query = _context.Projects
                .Where(p => p.UserId == userId)
                .OrderBy(p => p.Id);


            // count
            var total = await query.CountAsync();

            // pagination
            var items = await query.Skip((page - 1) * limit).Take(limit).ToListAsync();

            return new PagedResult<ProjectResponse>
            {
                Items = items.Select(ToResponse).ToList(),
                Page = page,
                Limit = limit,
                TotalCount = total
            };
        }

        public async Task<ProjectResponse> GetByIdAsync(int userId, int id)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                throw ApiException.NotFound($"Project with id {id} was not found.");
            }

            return ToResponse(project);
        }

        public async Task<ProjectResponse> UpdateAsync(int userId, int id, UpdateProjectRequest request)
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                throw ApiException.NotFound($"Project with id {id} was not found.");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
                throw ApiException.BadRequest("Project name is required.");

            if (!string.Equals(project.Name, request.Name, StringComparison.Ordinal))
            {
                var nameTaken = await _context.Projects.AnyAsync(p => p.UserId == userId && p.Name == request.Name && p.Id != id);
                if (nameTaken)
                    throw ApiException.Conflict($"A project named '{request.Name}' already exists.");
            }

            project.Name = request.Name.Trim();
            project.Description = request.Description;
            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return ToResponse(project);
        }

        public async Task DeleteAsync(int userId, int id)
        {
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);

            if (project == null)
            {
                throw ApiException.NotFound($"Project with id {id} was not found.");
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted project {ProjectId} ({ProjectName}) and cascaded its tasks", id, project.Name);
        }

        private static ProjectResponse ToResponse(Project p)
        {
            return new ProjectResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}