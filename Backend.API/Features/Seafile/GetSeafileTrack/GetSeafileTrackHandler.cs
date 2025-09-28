using AutoMapper;
using Backend.API.Features.Seafile.Models;
using Backend.API.Features.Seafile.Repositories;
using MediatR;

namespace Backend.API.Features.Seafile.GetSeafileTrack;

public sealed class GetSeafileTrackHandler : IRequestHandler<GetSeafileTrackQuery, SeafileTrackDto?>
{
    private readonly ISeafileTrackRepository _repository;
    private readonly IMapper _mapper;

    public GetSeafileTrackHandler(ISeafileTrackRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SeafileTrackDto?> Handle(GetSeafileTrackQuery request, CancellationToken cancellationToken)
    {
        var track = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return track == null ? null : _mapper.Map<SeafileTrackDto>(track);
    }
}
