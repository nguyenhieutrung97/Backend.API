using Backend.API.Features.Users.Models;
using Backend.API.Features.Users.Store;
using MediatR;

namespace Backend.API.Features.Users.ImportUser
{
    public sealed class ImportUserHandler : IRequestHandler<ImportUserCommand>
    {
        private readonly IUserMongoStore _store;
        public ImportUserHandler(IUserMongoStore store)
        {
            _store = store;
        }
        public async Task Handle(ImportUserCommand request, CancellationToken cancellationToken)
        {
            await _store.InsertAsync(request.User, cancellationToken);
        }
    }
}
