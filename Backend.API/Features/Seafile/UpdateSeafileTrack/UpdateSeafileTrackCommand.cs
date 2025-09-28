using Backend.API.Features.Seafile.Models;
using MediatR;

namespace Backend.API.Features.Seafile.UpdateSeafileTrack;

public sealed record class UpdateSeafileTrackCommand(string Id, string Title, string Src, string Folder) : IRequest<SeafileTrackDto?>;
