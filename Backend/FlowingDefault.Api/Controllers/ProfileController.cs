using FlowingDefault.Core.Services;
using FlowingDefault.Core;
using Microsoft.AspNetCore.Mvc;
using FlowingDefault.Core.Dtos;

namespace FlowingDefault.Api.Controllers
{
    [Route("[controller]")]
    public class ProfileController : AuthorizeController
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly ProfileService _profileService;
        private readonly UserService _userService;

        public ProfileController(ILogger<ProfileController> logger, UserService userService, ProfileService profileService) : base(logger)
        {
            _logger = logger;
            _profileService = profileService;
            _userService = userService;
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User if found, NotFound if not found</returns>
        [HttpGet]
        public async Task<ActionResult<UserDto>> Get()
        {
            var currentUserId = GetCurrentUserId();

            try
            {
                var userDto = await _userService.GetById(currentUserId);
                return Ok(userDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", currentUserId);
                return StatusCode(500, "An error occurred while retrieving the user");
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="dto">Updated user data</param>
        /// <returns>Updated user</returns>
        [HttpPut]
        public async Task<ActionResult<UserDto>> Update([FromBody] UserDto dto)
        {
            var currentUserId = GetCurrentUserId();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.Id = currentUserId;
                await _profileService.Save(dto);
                
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
            }
            catch (FlowingDefaultException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while updating user {UserId}", currentUserId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", currentUserId);
                return StatusCode(500, "An error occurred while updating the user");
            }
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="dto">Updated user data</param>
        /// <returns>Updated user</returns>
        [HttpPut("Password")]
        public async Task<ActionResult<UserDto>> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var currentUserId = GetCurrentUserId();

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                dto.Id = currentUserId;
                await _profileService.ChangePassword(currentUserId, dto.NewPassword);

                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                return Unauthorized(ex.Message);
            }
            catch (FlowingDefaultException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while updating user {UserId}", currentUserId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", currentUserId);
                return StatusCode(500, "An error occurred while updating the user");
            }
        }
    }
}