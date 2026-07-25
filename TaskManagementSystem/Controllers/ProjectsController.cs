using Microsoft.AspNetCore.Mvc;
using TaskManagementSystem.DTOs.Projects.Requests;
using TaskManagementSystem.Services.IServices;

namespace TaskManagementSystem.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // POST: api/projects
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            var project = await _projectService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetProjectById),
                new { id = project.Id },
                project);
        }

        // GET: api/projects
        [HttpGet]
        public async Task<IActionResult> GetAllProjects(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 10)
        {
            var projects = await _projectService.GetAllAsync(page, limit);

            return Ok(projects);
        }

        // GET: api/projects/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await _projectService.GetByIdAsync(id);

            return Ok(project);
        }

        // PUT: api/projects/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(
            int id,
            [FromBody] UpdateProjectRequest request)
        {
            var project = await _projectService.UpdateAsync(id, request);

            return Ok(project);
        }

        // DELETE: api/projects/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await _projectService.DeleteAsync(id);

            return NoContent();
        }
    }
}
