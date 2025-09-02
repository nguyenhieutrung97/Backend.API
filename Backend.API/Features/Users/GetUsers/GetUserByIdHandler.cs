using AutoMapper;
using Backend.API.Features.Users.Models;
using Backend.API.Features.Users.Store;
using MediatR;

namespace Backend.API.Features.Users.GetUsers
{
    public sealed class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, User?>
    {
        private readonly IUserMongoStore _store;
        private readonly IMapper _mapper;

        public GetUserByIdHandler(IUserMongoStore store, IMapper mapper)
        {
            _store = store;
            _mapper = mapper;
        }

        public async Task<User?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _store.GetByIdAsync(request.Id, cancellationToken);
            var dto = user is null ? null : _mapper.Map<User>(user);
            return dto;
        }
    }
}
