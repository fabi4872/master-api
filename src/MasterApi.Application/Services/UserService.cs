using MasterApi.Application.Abstractions.Authentication;
using MasterApi.Application.Users.Responses;
using System.Text.RegularExpressions;
using MasterApi.Application.Abstractions.Persistence;
using MasterApi.Application.Abstractions.Services;
using MasterApi.Application.Users.Requests;
using MasterApi.Domain.Core;
using MasterApi.Domain.Entities;
using MasterApi.Domain.Errors;
using MasterApi.Application.Auditing;
using MasterApi.Application.Abstractions.Auditing;

namespace MasterApi.Application.Services;

public partial class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly IBusinessAuditService _businessAuditService;

    public UserService(IUserRepository userRepository, IJwtProvider jwtProvider, IPasswordHasherService passwordHasher, IBusinessAuditService businessAuditService)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
        _passwordHasher = passwordHasher;
        _businessAuditService = businessAuditService;
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

        var (hash, salt) = _passwordHasher.Hash(request.Password);

        var user = new User(Guid.NewGuid(), request.Email, request.Name, hash, salt, request.Role);
        
        _userRepository.Add(user);

        await _businessAuditService.WriteAsync(new BusinessAuditEvent
        {
            EventType = "UserCreated",
            TimestampUtc = DateTime.UtcNow,
            UserId = user.Id,
            Metadata = { { "Email", user.Email }, { "Role", user.Role.ToString() } },
            CorrelationId = System.Diagnostics.Activity.Current?.Id ?? string.Empty
        });

        return user;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var correlationId = System.Diagnostics.Activity.Current?.Id ?? string.Empty;

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            await _businessAuditService.WriteAsync(new BusinessAuditEvent
            {
                EventType = "UserLoginFailed",
                TimestampUtc = DateTime.UtcNow,
                Metadata = { { "Reason", "EmailRequired" } },
                CorrelationId = correlationId
            });
            return Result.Failure<LoginResponse>(DomainErrors.User.EmailRequired);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            await _businessAuditService.WriteAsync(new BusinessAuditEvent
            {
                EventType = "UserLoginFailed",
                TimestampUtc = DateTime.UtcNow,
                Metadata = { { "Email", request.Email }, { "Reason", "PasswordRequired" } },
                CorrelationId = correlationId
            });
            return Result.Failure<LoginResponse>(DomainErrors.User.PasswordRequired);
        }

        var user = await _userRepository.GetUserByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            await _businessAuditService.WriteAsync(new BusinessAuditEvent
            {
                EventType = "UserLoginFailed",
                TimestampUtc = DateTime.UtcNow,
                Metadata = { { "Email", request.Email }, { "Reason", "InvalidCredentials" } },
                CorrelationId = correlationId
            });
            return Result.Failure<LoginResponse>(DomainErrors.User.InvalidCredentials);
        }

        if (!_passwordHasher.Verify(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            await _businessAuditService.WriteAsync(new BusinessAuditEvent
            {
                EventType = "UserLoginFailed",
                TimestampUtc = DateTime.UtcNow,
                UserId = user.Id,
                Metadata = { { "Email", request.Email }, { "Reason", "InvalidCredentials" } },
                CorrelationId = correlationId
            });
            return Result.Failure<LoginResponse>(DomainErrors.User.InvalidCredentials);
        }

        var token = _jwtProvider.Generate(user);
        
        await _businessAuditService.WriteAsync(new BusinessAuditEvent
        {
            EventType = "UserLoginSucceeded",
            TimestampUtc = DateTime.UtcNow,
            UserId = user.Id,
            Metadata = { { "Email", user.Email } },
            CorrelationId = correlationId
        });

        return new LoginResponse(token.AccessToken, token.ExpiresAt);
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();
}
