using Backend.API.Features.Seafile.Repositories;
using MediatR;

namespace Backend.API.Features.Seafile.DeleteSeafileTrack;

public sealed class DeleteSeafileTrackHandler : IRequestHandler<DeleteSeafileTrackCommand, bool>
{
    private readonly ISeafileTrackRepository _repository;

    public DeleteSeafileTrackHandler(ISeafileTrackRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteSeafileTrackCommand request, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(request.Id, cancellationToken);
    }
}
