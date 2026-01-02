using MasterApi.Domain.Entities;

namespace MasterApi.Application.Users.Requests;

public sealed record CreateUserRequest(string Name, string Email, string Password, UserRole Role);
