using Backend.API.Features.Users.GetUsers;
using Backend.API.Features.Users.Models;
using FluentValidation;
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

    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserFilter filter)
    {
        var result = await _validator.ValidateAsync(filter);
        if (!result.IsValid)
        {
            var details = new ValidationProblemDetails(result.ToDictionary());
            return ValidationProblem(details);
        }

        var paged = await _mediator.Send(new GetUsersQuery(filter));
        return Ok(paged);
    }
}