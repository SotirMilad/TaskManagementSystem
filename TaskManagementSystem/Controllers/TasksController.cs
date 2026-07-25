using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs.Common;
using TaskManagementSystem.DTOs.Tasks.Requests;
using TaskManagementSystem.Services.IServices;

namespace TaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // POST: api/projects/{projectId}/tasks
        [HttpPost("/api/projects/{projectId}/tasks")]
        public async Task<IActionResult> CreateTask(
            int projectId,
            [FromBody] CreateTaskRequest request)
        {
            var task = await _taskService.CreateAsync(projectId, request);

            return CreatedAtAction(
                nameof(GetTaskById),
                new { id = task.Id },
                task);
        }

        // GET: api/projects/{projectId}/tasks
        [HttpGet("/api/projects/{projectId}/tasks")]
        public async Task<IActionResult> GetProjectTasks(
            int projectId,
            [FromQuery] TaskQueryParameters query)
        {
            var tasks = await _taskService.GetByProjectAsync(projectId, query);

            return Ok(tasks);
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<IActionResult> GetAllTasks(
            [FromQuery] TaskQueryParameters query)
        {
            var tasks = await _taskService.GetAllAsync(query);

            return Ok(tasks);
        }

        // GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetByIdAsync(id);

            return Ok(task);
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(
            int id,
            [FromBody] UpdateTaskRequest request)
        {
            var task = await _taskService.UpdateAsync(id, request);

            return Ok(task);
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _taskService.DeleteAsync(id);

            return NoContent();
        }
    }
}