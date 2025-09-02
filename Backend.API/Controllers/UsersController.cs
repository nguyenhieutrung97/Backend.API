using Backend.API.Features.Users.GetUsers;
using Backend.API.Features.Users.ImportUser;
using Backend.API.Features.Users.Models;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IValidator<UserFilter> _validator;

    public UsersController(IMediator mediator, IValidator<UserFilter> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    // GET /api/users
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserFilter filter, CancellationToken ct)
    {
        ValidationResult result = await _validator.ValidateAsync(filter, ct);
        if (!result.IsValid)
        {
            ValidationProblemDetails details = new ValidationProblemDetails(result.ToDictionary());
            return ValidationProblem(details);
        }

        Paged<User> paged = await _mediator.Send(new GetUsersQuery(filter), ct);
        return Ok(paged);
    }

    // GET /api/users/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken ct)
    {
        User? user = await _mediator.Send(new GetUserByIdQuery(id), ct);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    // POST /api/users
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] User user, CancellationToken ct)
    {
        User createdUser = await _mediator.Send(new CreateUserCommand(user), ct);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    // PUT /api/users/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User user, CancellationToken ct)
    {
        bool updated = await _mediator.Send(new UpdateUserCommand(id, user), ct);
        if (!updated)
            return NotFound();
        return NoContent();
    }

    // DELETE /api/users/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        bool deleted = await _mediator.Send(new DeleteUserCommand(id), ct);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}