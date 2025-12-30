using MasterApi.Application.Abstractions.Persistence;
using MasterApi.Application.Abstractions.Services;
using MasterApi.Domain.Core;
using MasterApi.Domain.Entities;
using MasterApi.Domain.Errors;

namespace MasterApi.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<User>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
        {
            return Result.Failure<User>(DomainErrors.Request.ValidationError);
        }

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return Result.Failure<User>(DomainErrors.User.NotFound);
        }

        return user;
    }
}