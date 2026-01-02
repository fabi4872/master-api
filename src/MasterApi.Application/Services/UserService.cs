using System.Text.RegularExpressions;
using MasterApi.Application.Abstractions.Persistence;
using MasterApi.Application.Abstractions.Services;
using MasterApi.Application.Users.Requests;
using MasterApi.Domain.Core;
using MasterApi.Domain.Entities;
using MasterApi.Domain.Errors;

namespace MasterApi.Application.Services;

public partial class UserService : IUserService
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

    public async Task<Result<User>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result.Failure<User>(DomainErrors.User.NameRequired);
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure<User>(DomainErrors.User.EmailRequired);
        }
        
        if (!EmailRegex().IsMatch(request.Email))
        {
            return Result.Failure<User>(DomainErrors.User.EmailInvalid);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Result.Failure<User>(DomainErrors.User.PasswordRequired);
        }

        if (!await _userRepository.IsEmailUniqueAsync(request.Email, cancellationToken))
        {
            return Result.Failure<User>(DomainErrors.User.EmailAlreadyExists);
        }

        var user = new User(Guid.NewGuid(), request.Email, request.Name, request.Password);
        
        _userRepository.Add(user);

        return user;
    }

    public async Task<Result<User>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure<User>(DomainErrors.User.EmailRequired);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return Result.Failure<User>(DomainErrors.User.PasswordRequired);
        }

        var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<User>(DomainErrors.User.InvalidCredentials);
        }

        // TODO: Replace with password hashing and comparison
        if (user.Password != request.Password)
        {
            return Result.Failure<User>(DomainErrors.User.InvalidCredentials);
        }

        return user;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
