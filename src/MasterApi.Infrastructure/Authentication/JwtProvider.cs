using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MasterApi.Application.Abstractions.Authentication;
using MasterApi.Application.Authentication;
using MasterApi.Application.Authorization;
using MasterApi.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

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
        var claims = new List<Claim>
        {
            new("userId", user.Id.ToString()),
            new("email", user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        // TODO: Permissions should come from a database based on roles
        if (user.Role == UserRole.Admin)
        {
            claims.Add(new Claim("permission", Permissions.UsersRead));
            claims.Add(new Claim("permission", Permissions.UsersCreate));
            claims.Add(new Claim("permission", Permissions.UsersUpdate));
            claims.Add(new Claim("permission", Permissions.UsersDelete));
        }
        else if (user.Role == UserRole.User)
        {
            claims.Add(new Claim("permission", Permissions.UsersRead));
        }

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

    public RefreshToken GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        // TODO: Persist refresh token in DB in the future
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        };
    }
}
