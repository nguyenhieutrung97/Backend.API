using Backend.API.Features.Seafile.Models;
using MediatR;

namespace Backend.API.Features.Seafile.CreateSeafileTrack;

public sealed record class CreateSeafileTrackCommand(string Title, string Src, string Folder) : IRequest<SeafileTrackDto>;
