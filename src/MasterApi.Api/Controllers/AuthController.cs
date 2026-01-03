using MasterApi.Application.Abstractions.Services;
using MasterApi.Application.Users.Requests;
using MasterApi.Application.Users.Responses;
using MasterApi.Domain.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MasterApi.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ApiControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _userService.RefreshTokenAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error == DomainErrors.Auth.RefreshTokenExpired || result.Error == DomainErrors.Auth.InvalidRefreshToken
                ? Unauthorized(CreateProblemDetails("Unauthorized", StatusCodes.Status401Unauthorized, result.Error))
                : HandleFailure(result);
        }

        return Ok(result.Value);
    }
}
