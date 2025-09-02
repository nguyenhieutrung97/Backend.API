using Backend.API.Features.Users.Models;
using MediatR;

namespace Backend.API.Features.Users.GetUsers
{
    public sealed record GetUsersQuery(UserFilter Filter) : IRequest<Paged<User>>;

    public sealed record Paged<T>(IReadOnlyList<T> Items, int Page, int PageSize, int Total);
}
