using AutoMapper;
using Backend.API.Common;
using Backend.API.Features.Seafile.Models;
using Backend.API.Features.Seafile.Repositories;
using MediatR;

namespace Backend.API.Features.Seafile.GetSeafileTracks;

public sealed class GetSeafileTracksHandler : IRequestHandler<GetSeafileTracksQuery, Paged<SeafileTrackDto>>
{
    private readonly ISeafileTrackRepository _repository;
    private readonly IMapper _mapper;

    public GetSeafileTracksHandler(ISeafileTrackRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Paged<SeafileTrackDto>> Handle(GetSeafileTracksQuery request, CancellationToken cancellationToken)
    {
        var pagedTracks = await _repository.GetPagedAsync(request.Filter, cancellationToken);
        
        return new Paged<SeafileTrackDto>(
            _mapper.Map<List<SeafileTrackDto>>(pagedTracks.Items),
            pagedTracks.Page,
            pagedTracks.PageSize,
            pagedTracks.Total);
    }
}
