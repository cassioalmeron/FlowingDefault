using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlowingDefault.Api.DTOs;
using FlowingDefault.Core;
using Microsoft.IdentityModel.Tokens;

namespace FlowingDefault.Api.Services;

public class JwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(JwtSettings jwtSettings) =>
        _jwtSettings = jwtSettings;

    public LoginResponseDto GenerateToken(string userId, string username, string name)
    {
        if (_jwtSettings.Key.Length < 32)
            throw new FlowingDefaultException("The JWT Key must have at least 32 characters.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            //new Claim(JwtRegisteredClaimNames.Email, email),
            //new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expires = DateTime.Now.AddMinutes(_jwtSettings.ExpiryInMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);

        var response = new LoginResponseDto
        {
            Name = name,
            IsAdmin = userId == "1",
            Token = generatedToken,
            Expires = expires
        };

        return response;
    }
}