using Backend.API.Features.Users.Models;
using MediatR;

namespace Backend.API.Features.Users.ImportUser
{
    public sealed record UpdateUserCommand(Guid Id, User User) : IRequest<bool>;
}
