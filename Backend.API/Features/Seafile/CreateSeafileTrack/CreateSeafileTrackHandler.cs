using AutoMapper;
using Backend.API.Features.Seafile.Models;
using Backend.API.Features.Seafile.Repositories;
using MediatR;
using MongoDB.Bson;

namespace Backend.API.Features.Seafile.CreateSeafileTrack;

public sealed class CreateSeafileTrackHandler : IRequestHandler<CreateSeafileTrackCommand, SeafileTrackDto>
{
    private readonly ISeafileTrackRepository _repository;
    private readonly IMapper _mapper;

    public CreateSeafileTrackHandler(ISeafileTrackRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SeafileTrackDto> Handle(CreateSeafileTrackCommand request, CancellationToken cancellationToken)
    {
        var track = new SeafileTrack
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Title = request.Title,
            Src = request.Src,
            Folder = request.Folder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdTrack = await _repository.CreateAsync(track, cancellationToken);
        return _mapper.Map<SeafileTrackDto>(createdTrack);
    }
}
