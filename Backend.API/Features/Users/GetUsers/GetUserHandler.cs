using AutoMapper;
using Backend.API.Features.Users.Models;
using Backend.API.Common;
using Backend.API.Infrastructure.Data;
using MediatR;

namespace Backend.API.Features.Users.GetUsers
{
    public sealed class GetUsersHandler : IRequestHandler<GetUsersQuery, Paged<UserDto>>
    {
        private readonly IUserStore _store;
        private readonly IMapper _mapper;

        public GetUsersHandler(IUserStore store, IMapper mapper)
        {
            _store = store;
            _mapper = mapper;
        }

        public Task<Paged<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
        {
            var f = request.Filter;

            var q = _store.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(f.Search))
            {
                var s = f.Search.Trim();
                q = q.Where(u =>
                    (u.FirstName + " " + u.LastName).Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(s, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(f.Country))
                q = q.Where(u => u.Country.Equals(f.Country, StringComparison.OrdinalIgnoreCase));

            q = (f.Sort, f.Order) switch
            {
                // Age: oldest first when desc (earliest birthdate), youngest first when asc
                ("age", "desc") => q.OrderBy(u => u.BirthDate),
                ("age", _) => q.OrderByDescending(u => u.BirthDate),
                ("country", "desc") => q.OrderByDescending(u => u.Country),
                ("country", _) => q.OrderBy(u => u.Country),
                ("name", "desc") => q.OrderByDescending(u => u.LastName).ThenByDescending(u => u.FirstName),
                _ => q.OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
            };

            var total = q.Count();
            var items = q.Skip((f.Page - 1) * f.PageSize).Take(f.PageSize).ToList();

            var dto = _mapper.Map<List<UserDto>>(items);
            return Task.FromResult(new Paged<UserDto>(dto, f.Page, f.PageSize, total));
        }
    }
}
