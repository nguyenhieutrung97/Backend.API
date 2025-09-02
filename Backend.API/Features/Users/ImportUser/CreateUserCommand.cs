using Backend.API.Features.Users.Models;
using MediatR;

namespace Backend.API.Features.Users.ImportUser
{
    public sealed record CreateUserCommand(User User) : IRequest<User>;
}
