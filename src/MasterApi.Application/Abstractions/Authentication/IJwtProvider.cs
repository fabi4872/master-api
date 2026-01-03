using MasterApi.Application.Authentication;
using MasterApi.Domain.Entities;

namespace MasterApi.Application.Abstractions.Authentication;

public interface IJwtProvider
{
    (string AccessToken, DateTime ExpiresAt) Generate(User user);
    RefreshToken GenerateRefreshToken();
}
