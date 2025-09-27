using Backend.API.Features.Users.GetUsers;
using Backend.API.Features.Users.Models;
using Backend.API.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(Paged<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUsers([FromQuery] UserFilter filter, CancellationToken ct)
        => Ok(await _mediator.Send(new GetUsersQuery(filter), ct));
}