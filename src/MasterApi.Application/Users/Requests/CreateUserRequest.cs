namespace MasterApi.Application.Users.Requests;

public sealed record CreateUserRequest(string Name, string Email, string Password);
