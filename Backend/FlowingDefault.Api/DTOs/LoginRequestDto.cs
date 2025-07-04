namespace FlowingDefault.Api.DTOs;
/// <summary>
/// Login request model
/// </summary>
public record LoginRequestDto
{
    /// <summary>
    /// Username for authentication
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// Password for authentication
    /// </summary>
    public string Password { get; init; } = string.Empty;
}