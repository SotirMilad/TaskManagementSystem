using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Context;
using TaskManagementSystem.DTOs.Common;
using TaskManagementSystem.DTOs.Tasks.Requests;
using TaskManagementSystem.DTOs.Tasks.Responses;
using TaskManagementSystem.Enums;
using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Models;
using TaskManagementSystem.Services.IServices;

namespace TaskManagementSystem.Services.ImplementationServices
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ApplicationDBContext context, ILogger<TaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TaskResponse> CreateAsync(int userId, int projectId, CreateTaskRequest request)
        {
            // check project exists
            var project = await _context.Projects
                .Where(p => p.DeletedAt == null)
                .FirstOrDefaultAsync(p => p.Id == projectId && p.UserId == userId);

            if (project == null)
                throw ApiException.NotFound($"Project with ID {projectId} was not found.");

            // validate title
            if (string.IsNullOrWhiteSpace(request.Title))
                throw ApiException.BadRequest("Task title is required.");

            // validate due date
            if (request.DueDate.HasValue &&
                request.DueDate.Value < DateOnly.FromDateTime(DateTime.Today))
            {
                throw ApiException.BadRequest("Due date cannot be in the past.");
            }

            // create the taask
            var task = new TaskItem
            {
                ProjectId = projectId,
                Title = request.Title.Trim(),
                Description = request.Description,
                Status = ParseStatus(request.Status),
                Priority = ParsePriority(request.Priority),
                DueDate = request.DueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            return MapToResponse(task, project.Name);
        }

        public async Task<PagedResult<TaskResponse>> GetAllAsync(int userId, TaskQueryParameters query)
        {
            // validate pagination
            query.Page = Math.Max(query.Page, 1);
            query.Limit = Math.Clamp(query.Limit, 1, 100);

            var tasksQuery = _context.Tasks
                .Where(t => t.DeletedAt == null)
                .Include(t => t.Project)
                .Where(t => t.Project.UserId == userId)
                .AsNoTracking();

            tasksQuery = ApplySearch(tasksQuery, query.Search);
            tasksQuery = ApplyFilters(tasksQuery, query);
            tasksQuery = ApplySorting(tasksQuery, query);

            var totalCount = await tasksQuery.CountAsync();

            var tasks = await tasksQuery
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync();

            var result = new PagedResult<TaskResponse>
            {
                Items = tasks
                    .Select(t => MapToResponse(t, t.Project!.Name))
                    .ToList(),

                Page = query.Page,
                Limit = query.Limit,
                TotalCount = totalCount
            };

            return result;
        }

        public async Task<PagedResult<TaskResponse>> GetByProjectAsync(int userId, int projectId, TaskQueryParameters query)
        {
            // check project exists
            bool projectExists = await _context.Projects
                .Where(p => p.DeletedAt == null)
                .AnyAsync(p => p.Id == projectId && p.UserId == userId);

            if (!projectExists)
            {
                throw ApiException.NotFound($"Project with ID {projectId} was not found.");
            }

            // validate pagination
            query.Page = Math.Max(query.Page, 1);
            query.Limit = Math.Clamp(query.Limit, 1, 100);

            var tasksQuery = _context.Tasks
                .Where(t => t.DeletedAt == null)
                .Include(t => t.Project)
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId);

            tasksQuery = ApplySearch(tasksQuery, query.Search);
            tasksQuery = ApplyFilters(tasksQuery, query);
            tasksQuery = ApplySorting(tasksQuery, query);

            var totalCount = await tasksQuery.CountAsync();

            var tasks = await tasksQuery
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .ToListAsync();

            return new PagedResult<TaskResponse>
            {
                Items = tasks
                    .Select(t => MapToResponse(t, t.Project!.Name))
                    .ToList(),

                Page = query.Page,
                Limit = query.Limit,
                TotalCount = totalCount
            };
        }

        public async Task<TaskResponse> GetByIdAsync(int userId, int id)
        {
            var task = await _context.Tasks
                .Where(t => t.DeletedAt == null)
                .Include(t => t.Project)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && t.Project!.UserId == userId);

            if (task == null)
            {
                throw ApiException.NotFound($"Task with ID {id} was not found.");
            }

            return MapToResponse(task, task.Project!.Name);
        }

        public async Task<TaskResponse> UpdateAsync(int userId, int id, UpdateTaskRequest request)
        {
            var task = await _context.Tasks
                .Where(t => t.DeletedAt == null)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project!.UserId == userId);

            if (task == null)
            {
                throw ApiException.NotFound($"Task with ID {id} was not found.");
            }

            // validate title
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw ApiException.BadRequest("Task title is required.");
            }

            // validate due date
            if (request.DueDate.HasValue &&
                request.DueDate.Value < DateOnly.FromDateTime(DateTime.Today))
            {
                throw ApiException.BadRequest("Due date cannot be in the past.");
            }

            var newStatus = ParseStatus(request.Status);
            var newPriority = ParsePriority(request.Priority);

            // log unusual transition of task status
            if (task.Status == TaskState.Done &&
                newStatus == TaskState.Todo)
            {
                _logger.LogWarning(
                    "Task {TaskId} changed from Done back to Todo.",
                    task.Id);
            }

            task.Title = request.Title.Trim();
            task.Description = request.Description;
            task.Status = newStatus;
            task.Priority = newPriority;
            task.DueDate = request.DueDate;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponse(task, task.Project!.Name);
        }

        public async Task DeleteAsync(int userId, int id)
        {
            var task = await _context.Tasks
                .Where(t => t.DeletedAt == null)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.Id == id && t.Project!.UserId == userId);

            if (task == null)
            {
                throw ApiException.NotFound($"Task with ID {id} was not found.");
            }


            // soft delete
            task.DeletedAt = DateTime.UtcNow;

            //hard delete
            //_context.Tasks.Remove(task);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted task {TaskId} ({TaskTitle}) from project {ProjectId}", task.Id, task.Title, task.ProjectId);
        }

        private IQueryable<TaskItem> ApplyFilters(
            IQueryable<TaskItem> query,
            TaskQueryParameters parameters)
        {
            if (!string.IsNullOrWhiteSpace(parameters.Status))
            {
                var status = ParseStatus(parameters.Status);
                query = query.Where(t => t.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Priority))
            {
                var priority = ParsePriority(parameters.Priority);
                query = query.Where(t => t.Priority == priority);
            }

            if (parameters.DueDateFrom.HasValue)
            {
                query = query.Where(t =>
                    t.DueDate.HasValue &&
                    t.DueDate.Value >= parameters.DueDateFrom.Value);
            }

            if (parameters.DueDateTo.HasValue)
            {
                query = query.Where(t =>
                    t.DueDate.HasValue &&
                    t.DueDate.Value <= parameters.DueDateTo.Value);
            }

            return query;
        }

        private IQueryable<TaskItem> ApplySorting(
            IQueryable<TaskItem> query,
            TaskQueryParameters parameters)
        {
            bool descending = parameters.SortDirection.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase);

            switch (parameters.SortBy?.ToLower())
            {
                case "duedate":
                    query = descending
                        ? query.OrderByDescending(t => t.DueDate)
                        : query.OrderBy(t => t.DueDate);
                    break;

                case "priority":
                    query = descending
                        ? query.OrderByDescending(t => t.Priority)
                        : query.OrderBy(t => t.Priority);
                    break;

                case "createdat":
                    query = descending
                        ? query.OrderByDescending(t => t.CreatedAt)
                        : query.OrderBy(t => t.CreatedAt);
                    break;

                default:
                    query = query.OrderBy(t => t.Id);
                    break;
            }

            return query;
        }

        private IQueryable<TaskItem> ApplySearch(
            IQueryable<TaskItem> query,
            string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return query;
            }

            search = search.Trim().ToLower();

            return query.Where(t =>
                t.Title.ToLower().Contains(search) ||
                (t.Description != null &&
                 t.Description.ToLower().Contains(search)));
        }

        private TaskState ParseStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return TaskState.Todo;
            }

            if (Enum.TryParse<TaskState>(status, true, out var taskStatus))
            {
                return taskStatus;
            }

            throw ApiException.BadRequest("Invalid task status.");
        }

        private TaskPriority ParsePriority(string? priority)
        {
            if (string.IsNullOrWhiteSpace(priority))
            {
                return TaskPriority.Medium;
            }

            if (Enum.TryParse<TaskPriority>(priority, true, out var taskPriority))
            {
                return taskPriority;
            }

            throw ApiException.BadRequest("Invalid task priority.");
        }

        private TaskResponse MapToResponse(TaskItem task, string projectName)
        {
            return new TaskResponse
            {
                Id = task.Id,
                ProjectId = task.ProjectId,
                ProjectName = projectName,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}