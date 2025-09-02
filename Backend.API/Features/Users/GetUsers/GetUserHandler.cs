using AutoMapper;
using Backend.API.Features.Users.Models;
using Backend.API.Features.Users.Store;
using MediatR;

namespace Backend.API.Features.Users.GetUsers
{
    public sealed class GetUsersHandler : IRequestHandler<GetUsersQuery, Paged<User>>
    {
        private readonly IUserMongoStore _store;
        private readonly IMapper _mapper;

        public GetUsersHandler(IUserMongoStore store, IMapper mapper)
        {
            _store = store;
            _mapper = mapper;
        }

        public async Task<Paged<User>> Handle(GetUsersQuery request, CancellationToken ct)
        {
            UserFilter filter = request.Filter;

            IReadOnlyList<User> allUsers = await _store.GetAllAsync(ct);
            IEnumerable<User> query = allUsers;

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                string s = filter.Search.Trim();
                query = query.Where(u =>
                    $"{u.FirstName} {u.LastName}".Contains(s, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(s, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(filter.Country))
            {
                query = query.Where(u => string.Equals(u.Country, filter.Country, StringComparison.OrdinalIgnoreCase));
            }

            string sort = filter.Sort?.ToLowerInvariant() ?? "name";
            string order = filter.Order?.ToLowerInvariant() ?? "asc";

            query = (sort, order) switch
            {
                // Age asc: younger first => later BirthDate first (descending)
                ("age", "asc")      => query.OrderByDescending(u => u.BirthDate)
                                             .ThenBy(u => u.LastName).ThenBy(u => u.FirstName).ThenBy(u => u.Id),
                // Age desc: older first => earlier BirthDate first (ascending)
                ("age", _)          => query.OrderBy(u => u.BirthDate)
                                             .ThenBy(u => u.LastName).ThenBy(u => u.FirstName).ThenBy(u => u.Id),

                ("country", "desc") => query.OrderByDescending(u => u.Country)
                                             .ThenBy(u => u.LastName).ThenBy(u => u.FirstName).ThenBy(u => u.Id),
                ("country", _)      => query.OrderBy(u => u.Country)
                                             .ThenBy(u => u.LastName).ThenBy(u => u.FirstName).ThenBy(u => u.Id),

                ("name", "desc")    => query.OrderByDescending(u => u.LastName)
                                             .ThenByDescending(u => u.FirstName).ThenBy(u => u.Id),
                _                   => query.OrderBy(u => u.LastName)
                                             .ThenBy(u => u.FirstName).ThenBy(u => u.Id)
            };

            int total = query.Count();
            List<User> items = query.Skip((filter.Page - 1) * filter.PageSize)
                                    .Take(filter.PageSize)
                                    .ToList();

            List<User> dto = _mapper.Map<List<User>>(items);
            return new Paged<User>(dto, filter.Page, filter.PageSize, total);
        }
    }
}
