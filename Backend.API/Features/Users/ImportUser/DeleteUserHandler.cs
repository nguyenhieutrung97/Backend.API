using Backend.API.Features.Users.Models;
using Backend.API.Features.Users.Store;
using MediatR;

namespace Backend.API.Features.Users.ImportUser
{
    public sealed class DeleteUserHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly IUserMongoStore _store;
        public DeleteUserHandler(IUserMongoStore store)
        {
            _store = store;
        }
        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await _store.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
