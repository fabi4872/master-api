using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MasterApi.Application.Abstractions.Authentication;
using MasterApi.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MasterApi.Infrastructure.Authentication;

public class JwtProvider : IJwtProvider
{
    private readonly JwtSettings _jwtSettings;

    public JwtProvider(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public (string AccessToken, DateTime ExpiresAt) Generate(User user)
    {
        var claims = new Claim[]
        {
            new("userId", user.Id.ToString()),
            new("email", user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            null,
            DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials);

        var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

        return (tokenValue, token.ValidTo);
    }
}
