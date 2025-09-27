using Backend.API.Features.Users.Models;
using Backend.API.Common;
using MediatR;

namespace Backend.API.Features.Users.GetUsers
{
    public sealed record GetUsersQuery(UserFilter Filter) : IRequest<Paged<UserDto>>;
}
