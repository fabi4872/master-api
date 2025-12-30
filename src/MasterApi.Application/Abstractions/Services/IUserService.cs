using MasterApi.Domain.Core;
using MasterApi.Domain.Entities;

namespace MasterApi.Application.Abstractions.Services;

public interface IUserService
{
    Task<Result<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}