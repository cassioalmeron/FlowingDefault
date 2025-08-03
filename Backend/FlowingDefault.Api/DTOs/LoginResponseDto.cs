namespace FlowingDefault.Api.DTOs;

public class LoginResponseDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime Expires { get; set; } = DateTime.MaxValue;
    public bool IsAdmin { get; set; } = false;
    public string Token { get; set; } = string.Empty;
}