using MasterApi.Application.Authentication;

namespace MasterApi.Application.Users.Responses;

public sealed record LoginResponse(string AccessToken, DateTime ExpiresAt, RefreshToken RefreshToken);
