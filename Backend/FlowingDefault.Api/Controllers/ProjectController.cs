using FlowingDefault.Core.Models;
using FlowingDefault.Core.Services;
using FlowingDefault.Core;
using FlowingDefault.Core.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace FlowingDefault.Api.Controllers
{
    [Route("[controller]")]
    public class ProjectController : AuthorizeController
    {
        private readonly ILogger<ProjectController> _logger;
        private readonly ProjectService _projectService;

        public ProjectController(ILogger<ProjectController> logger, ProjectService projectService) : base(logger)
        {
            _logger = logger;
            _projectService = projectService;
        }

        /// <summary>
        /// Get all projects for the current user
        /// </summary>
        /// <returns>List of all projects for the current user</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetAll()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var projects = await _projectService.GetAll(currentUserId);
                return Ok(projects);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects for user");
                return StatusCode(500, "An error occurred while retrieving projects");
            }
        }

        /// <summary>
        /// Get project by ID for the current user
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns>Project if found, NotFound if not found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetById(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var project = await _projectService.GetById(id, currentUserId);
                
                if (project == null)
                    return NotFound($"Project with ID {id} not found");

                return Ok(project);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
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
        public async Task<ActionResult<Project>> Create([FromBody] ProjectDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUserId = GetCurrentUserId();
                await _projectService.Save(dto, currentUserId);
                
                return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
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
        public async Task<ActionResult<Project>> Update(int id, [FromBody] ProjectDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUserId = GetCurrentUserId();
                var existingProject = await _projectService.GetById(id, currentUserId);
                if (existingProject == null)
                    return NotFound($"Project with ID {id} not found");

                dto.Id = id;

                await _projectService.Save(dto, currentUserId);
                
                return Ok(dto);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
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
                var currentUserId = GetCurrentUserId();
                var deleted = await _projectService.Delete(id, currentUserId);
                
                if (!deleted)
                    return NotFound($"Project with ID {id} not found");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
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
        /// Test endpoint to verify token claims
        /// </summary>
        /// <returns>Current user information from token</returns>
        [HttpGet("test-token")]
        public ActionResult<object> TestToken()
        {
            try
            {
                // Get the authorization header
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(new { Error = "Authorization header not found or invalid format" });
                }

                // Extract the token from the Bearer header
                var token = authHeader.Substring("Bearer ".Length).Trim();
                
                // Decode the JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                
                var claims = jwtToken.Claims.Select(c => new { c.Type, c.Value }).ToList();
                var userId = GetCurrentUserId();
                
                return Ok(new
                {
                    UserId = userId,
                    AllClaims = claims,
                    Username = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.UniqueName)?.Value,
                    TokenInfo = new
                    {
                        Issuer = jwtToken.Issuer,
                        Audience = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Aud)?.Value,
                        ValidFrom = jwtToken.ValidFrom,
                        ValidTo = jwtToken.ValidTo
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Check if project exists for the current user
        /// </summary>
        /// <param name="id">Project ID</param>
        /// <returns>True if project exists, false otherwise</returns>
        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> ProjectExists(int id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var exists = await _projectService.ProjectExists(id, currentUserId);
                return Ok(exists);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if project {ProjectId} exists", id);
                return StatusCode(500, "An error occurred while checking project existence");
            }
        }
    }
} 