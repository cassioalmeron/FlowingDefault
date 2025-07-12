using FlowingDefault.Core.Models;
using FlowingDefault.Core.Services;
using FlowingDefault.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FlowingDefault.Api.DTOs;
using FlowingDefault.Core.Dtos;

namespace FlowingDefault.Api.Controllers
{
    [Route("[controller]")]
    public class UserController : AuthorizeController
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;

        public UserController(ILogger<UserController> logger, UserService userService) : base(logger)
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
                var currentUserId = GetCurrentUserId();
                
                // Prevent user from viewing other users' data
                if (id != currentUserId)
                {
                    _logger.LogWarning("User {CurrentUserId} attempted to view user {TargetUserId}", currentUserId, id);
                    return BadRequest("You can only view your own account");
                }

                var user = await _userService.GetById(id);
                
                if (user == null)
                    return NotFound($"User with ID {id} not found");

                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
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
        /// <param name="userDto">User data</param>
        /// <returns>Created user with ID</returns>
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUserId = GetCurrentUserId();
                if (currentUserId != 1)
                {
                    _logger.LogWarning("User {UserId} attempted to delete themselves", currentUserId);
                    return BadRequest("Only the Admin can create users.");
                }

                await _userService.Save(userDto);

                return Ok(userDto);
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
        /// <param name="userDto">Updated user data</param>
        /// <returns>Updated user</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var currentUserId = GetCurrentUserId();
                if (currentUserId != 1)
                {
                    _logger.LogWarning("User {UserId} attempted to delete themselves", currentUserId);
                    return BadRequest("Only the Admin can update users.");
                }

                var existingUser = await _userService.GetById(id);
                if (existingUser == null)
                    return NotFound($"User with ID {id} not found");

                userDto.Id = id;
                await _userService.Save(userDto);
                
                return Ok(userDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
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
                var currentUserId = GetCurrentUserId();

                if (currentUserId != 1)
                {
                    _logger.LogWarning("User {UserId} attempted to delete themselves", currentUserId);
                    return BadRequest("Only the Admin can delete users.");
                }

                // Prevent user from deleting themselves
                if (id == currentUserId)
                {
                    _logger.LogWarning("User {UserId} attempted to delete themselves", currentUserId);
                    return BadRequest("You cannot delete your own account.");
                }

                var deleted = await _userService.Delete(id);
                
                if (!deleted)
                    return NotFound($"User with ID {id} not found");

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
            }
            catch (FlowingDefaultException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", id);
                return StatusCode(500, "An error occurred while deleting the user");
            }
        }

        /// <summary>
        /// Get current authenticated user
        /// </summary>
        /// <returns>Current user information</returns>
        [HttpGet("me")]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var user = await _userService.GetById(currentUserId);
                
                if (user == null)
                    return NotFound("Current user not found");

                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user");
                return StatusCode(500, "An error occurred while retrieving current user");
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