using MasterApi.Application.Abstractions.Persistence;
using MasterApi.Domain.Entities;

namespace MasterApi.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private static readonly List<User> _users = new();

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Id == id && !u.IsDeleted);
        return Task.FromResult(user);
    }
    
    public Task<User?> GetDeletedByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Id == id && u.IsDeleted);
        return Task.FromResult(user);
    }

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = _users.FirstOrDefault(u => u.Email == email);
        return Task.FromResult(user);
    }

    public Task<bool> IsEmailUniqueAsync(string email, CancellationToken cancellationToken = default)
    {
        var isUnique = !_users.Exists(u => u.Email == email && !u.IsDeleted);
        return Task.FromResult(isUnique);
    }

    public void Add(User user)
    {
        _users.Add(user);
    }
}
