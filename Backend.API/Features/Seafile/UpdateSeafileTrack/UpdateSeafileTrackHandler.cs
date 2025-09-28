using AutoMapper;
using Backend.API.Features.Seafile.Models;
using Backend.API.Features.Seafile.Repositories;
using MediatR;

namespace Backend.API.Features.Seafile.UpdateSeafileTrack;

public sealed class UpdateSeafileTrackHandler : IRequestHandler<UpdateSeafileTrackCommand, SeafileTrackDto?>
{
    private readonly ISeafileTrackRepository _repository;
    private readonly IMapper _mapper;

    public UpdateSeafileTrackHandler(ISeafileTrackRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SeafileTrackDto?> Handle(UpdateSeafileTrackCommand request, CancellationToken cancellationToken)
    {
        var existingTrack = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (existingTrack == null)
        {
            return null;
        }

        var updatedTrack = existingTrack with
        {
            Title = request.Title,
            Src = request.Src,
            Folder = request.Folder,
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _repository.UpdateAsync(updatedTrack, cancellationToken);
        return _mapper.Map<SeafileTrackDto>(result);
    }
}
