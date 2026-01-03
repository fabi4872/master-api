using MasterApi.Application.Users.Requests;
using MasterApi.Application.Users.Responses;
using MasterApi.Domain.Core;
using MasterApi.Domain.Entities;

namespace MasterApi.Application.Abstractions.Services;

public interface IUserService
{
    Task<Result<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<User>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteUserAsync(Guid userId, Guid? performedByUserId, CancellationToken cancellationToken = default);
    Task<Result> RestoreUserAsync(Guid userId, Guid? performedByUserId, CancellationToken cancellationToken = default);
    Task<Result<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
}