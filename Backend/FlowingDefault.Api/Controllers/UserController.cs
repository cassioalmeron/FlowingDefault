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
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;

        public UserController(ILogger<UserController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>List of all users</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll()
        {
            try
            {
                var users = await _userService.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(500, "An error occurred while retrieving users");
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User if found, NotFound if not found</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            try
            {
                var user = await _userService.GetById(id);
                
                if (user == null)
                    return NotFound($"User with ID {id} not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user">User data</param>
        /// <returns>Created user with ID</returns>
        [HttpPost]
        public async Task<ActionResult<User>> Create([FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _userService.Save(user);
                
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (FlowingDefaultException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while creating user");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, "An error occurred while creating the user");
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="user">Updated user data</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update(int id, [FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (id != user.Id)
                    return BadRequest("ID in URL does not match ID in request body");

                var existingUser = await _userService.GetById(id);
                if (existingUser == null)
                    return NotFound($"User with ID {id} not found");

                await _userService.Save(user);
                
                return Ok(user);
            }
            catch (FlowingDefaultException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while updating user {UserId}", id);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", id);
                return StatusCode(500, "An error occurred while updating the user");
            }
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>No content if deleted, NotFound if not found</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _userService.Delete(id);
                
                if (!deleted)
                    return NotFound($"User with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return StatusCode(500, "An error occurred while deleting the user");
            }
        }

        /// <summary>
        /// Check if username exists
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>True if username exists, false otherwise</returns>
        [HttpGet("check-username/{username}")]
        public async Task<ActionResult<bool>> CheckUsername(string username)
        {
            try
            {
                var exists = await _userService.UsernameExists(username);
                return Ok(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking username {Username}", username);
                return StatusCode(500, "An error occurred while checking username");
            }
        }
    }
}