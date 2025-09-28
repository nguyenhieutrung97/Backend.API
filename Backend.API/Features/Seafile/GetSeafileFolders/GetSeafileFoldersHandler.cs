using Backend.API.Features.Seafile.Repositories;
using MediatR;
using MongoDB.Driver;

namespace Backend.API.Features.Seafile.GetSeafileFolders;

public sealed class GetSeafileFoldersHandler : IRequestHandler<GetSeafileFoldersQuery, List<string>>
{
    private readonly ISeafileTrackRepository _repository;

    public GetSeafileFoldersHandler(ISeafileTrackRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<string>> Handle(GetSeafileFoldersQuery request, CancellationToken cancellationToken)
    {
        return await _repository.GetDistinctFoldersAsync(cancellationToken);
    }
}
