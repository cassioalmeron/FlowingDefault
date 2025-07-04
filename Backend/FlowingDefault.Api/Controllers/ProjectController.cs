using FlowingDefault.Core.Models;
using FlowingDefault.Core.Services;
using FlowingDefault.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace FlowingDefault.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly ProjectService _projectService;

        public ProjectController(ILogger<ProjectController> logger, ProjectService projectService)
        {
            _logger = logger;
            _projectService = projectService;
        }

        /// <summary>
        /// Get all projects
        /// </summary>
        /// <returns>List of all projects</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            try
            {
                var projects = await _projectService.GetAll();
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all projects");
                return StatusCode(500, "An error occurred while retrieving projects");
            }
        }

        /// <summary>
        /// Get project by ID
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns>Project if found, NotFound if not found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetById(int id)
        {
            try
            {
                var project = await _projectService.GetById(id);
                
                if (project == null)
                    return NotFound($"Project with ID {id} not found");

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project with ID {ProjectId}", id);
                return StatusCode(500, "An error occurred while retrieving the project");
            }
        }

        /// <summary>
        /// Get projects by user ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of projects for the specified user</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Project>>> GetByUserId(int userId)
        {
            try
            {
                var projects = await _projectService.GetByUserId(userId);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user projects");
            }
        }

        /// <summary>
        /// Create a new project
        /// </summary>
        /// <param name="project">Project data</param>
        /// <returns>Created project with ID</returns>
        [HttpPost]
        public async Task<ActionResult<Project>> Create([FromBody] Project project)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _projectService.Save(project);
                
                return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
            }
            catch (FlowingDefaultException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while creating project");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, "An error occurred while creating the project");
            }
        }

        /// <summary>
        /// Update an existing project
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <param name="project">Updated project data</param>
        /// <returns>Updated project</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Project>> Update(int id, [FromBody] Project project)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != project.Id)
                    return BadRequest("ID in URL does not match ID in request body");

                var existingProject = await _projectService.GetById(id);
                if (existingProject == null)
                    return NotFound($"Project with ID {id} not found");

                await _projectService.Save(project);
                
                return Ok(project);
            }
            catch (FlowingDefaultException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while updating project {ProjectId}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project {ProjectId}", id);
                return StatusCode(500, "An error occurred while updating the project");
            }
        }

        /// <summary>
        /// Delete a project
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns>No content if deleted, NotFound if not found</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _projectService.Delete(id);
                
                if (!deleted)
                    return NotFound($"Project with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project {ProjectId}", id);
                return StatusCode(500, "An error occurred while deleting the project");
            }
        }

        /// <summary>
        /// Check if project name exists for a specific user
        /// </summary>
        /// <param name="projectName">Project name to check</param>
        /// <param name="userId">User ID</param>
        /// <returns>True if project name exists for the user, false otherwise</returns>
        [HttpGet("check-name/{projectName}/user/{userId}")]
        public async Task<ActionResult<bool>> CheckProjectNameForUser(string projectName, int userId)
        {
            try
            {
                var exists = await _projectService.ProjectNameExistsForUser(projectName, userId);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking project name {ProjectName} for user {UserId}", projectName, userId);
                return StatusCode(500, "An error occurred while checking project name");
            }
        }

        /// <summary>
        /// Check if project exists
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns>True if project exists, false otherwise</returns>
        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> ProjectExists(int id)
        {
            try
            {
                var exists = await _projectService.ProjectExists(id);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if project {ProjectId} exists", id);
                return StatusCode(500, "An error occurred while checking project existence");
            }
        }
    }
} 