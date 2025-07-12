using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using FlowingDefault.Core;

namespace FlowingDefault.Api.Controllers
{
    [ApiController]
    [Authorize]
    public abstract class AuthorizeController : ControllerBase
    {
        protected readonly ILogger _logger;

        protected AuthorizeController(ILogger logger) =>
            _logger = logger;

        /// <summary>
        /// Extract user ID from the current user's claims
        /// </summary>
        /// <returns>User ID as integer</returns>
        protected int GetCurrentUserId()
        {
            // Get the authorization header
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogError("Authorization header not found or invalid format");
                throw new UnauthorizedAccessException("Authorization header not found");
            }

            // Extract the token from the Bearer header
            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            try
            {
                // Decode the JWT token
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                
                // Find the user ID claim (sub claim)
                var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
                
                if (userIdClaim == null)
                {
                    // Log all available claims for debugging
                    var allClaims = jwtToken.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
                    _logger.LogError("User ID claim (sub) not found in token. Available claims: {Claims}", string.Join(", ", allClaims));
                    throw new UnauthorizedAccessException("User ID not found in token");
                }

                _logger.LogDebug("Found user ID claim: {UserIdClaim}", userIdClaim.Value);

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    _logger.LogError("Failed to parse user ID from token. Value: {UserIdValue}", userIdClaim.Value);
                    throw new UnauthorizedAccessException($"Invalid user ID format in token: {userIdClaim.Value}");
                }

                _logger.LogDebug("Successfully extracted user ID: {UserId}", userId);
                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decoding JWT token");
                throw new UnauthorizedAccessException("Invalid JWT token");
            }
        }
    }
} 