using MasterApi.Domain.Entities;

namespace MasterApi.Application.Abstractions.Persistence;

public interface IUserRepository
{
    // Returns a non-deleted user by ID.
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    // Returns a potentially deleted user by ID.
    Task<User?> GetDeletedByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default);
    void Add(User user);
}
