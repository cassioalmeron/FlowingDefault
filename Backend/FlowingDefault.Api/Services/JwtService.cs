using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlowingDefault.Core;
using Microsoft.IdentityModel.Tokens;

namespace FlowingDefault.Api.Services;

public class JwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(JwtSettings jwtSettings)
    {
        _jwtSettings = jwtSettings;
    }

    //public string GenerateToken(string userId, string email, string role)
    public string GenerateToken(string userId, string username)
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

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}