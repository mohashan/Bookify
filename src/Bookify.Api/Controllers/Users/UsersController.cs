﻿using Asp.Versioning;
using Bookify.Application.Users.GetLoggedInUser;
using Bookify.Application.Users.LogInUser;
using Bookify.Application.Users.RegisterUser;
using Bookify.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Users;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/users")]
public class UsersController : ControllerBase
{
    private readonly ISender sender;

    public UsersController(ISender sender)
    {
        this.sender = sender;
    }

    [HttpGet("me")]
    [MapToApiVersion(ApiVersions.V1)]
    //[Authorize(Roles = Roles.Registered)]
    [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetLoggedInUserV1(CancellationToken cancellationToken)
    {
        var query = new GetLoggedInUserQuery();

        var result = await sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Email, request.FirstName, request.LastName, request.Password);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [AllowAnonymous, HttpPost("login")]
    public async Task<IActionResult> LogIn(LogInUserRequest request, CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(request.Email, request.Password);

        var result = await sender.Send(command, cancellationToken);
        if (result.IsFailure)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);
    }
}
