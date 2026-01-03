namespace MasterApi.Application.Authentication;

public class RefreshToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
}
