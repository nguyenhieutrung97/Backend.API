using MediatR;

namespace Backend.API.Features.Users.ImportUser
{
    public sealed record DeleteUserCommand(Guid Id) : IRequest<bool>;
}
