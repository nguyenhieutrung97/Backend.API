using Backend.API.Features.Users.Models;
using Backend.API.Features.Users.Store;
using MediatR;

namespace Backend.API.Features.Users.ImportUser
{
    public sealed class UpdateUserHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly IUserMongoStore _store;
        public UpdateUserHandler(IUserMongoStore store)
        {
            _store = store;
        }
        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            return await _store.UpdateAsync(request.Id, request.User, cancellationToken);
        }
    }
}
