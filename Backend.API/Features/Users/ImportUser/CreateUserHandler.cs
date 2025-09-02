using Backend.API.Features.Users.Models;
using Backend.API.Features.Users.Store;
using MediatR;

namespace Backend.API.Features.Users.ImportUser
{
    public sealed class CreateUserHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IUserMongoStore _store;
        public CreateUserHandler(IUserMongoStore store)
        {
            _store = store;
        }
        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            return await _store.InsertAsync(request.User, cancellationToken);
        }
    }
}
