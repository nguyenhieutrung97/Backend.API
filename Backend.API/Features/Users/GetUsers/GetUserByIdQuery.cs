using Backend.API.Features.Users.Models;
using MediatR;

namespace Backend.API.Features.Users.GetUsers
{
    public sealed record GetUserByIdQuery(Guid Id) : IRequest<User?>;
}
