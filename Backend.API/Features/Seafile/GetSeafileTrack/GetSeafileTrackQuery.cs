using Backend.API.Features.Seafile.Models;
using MediatR;

namespace Backend.API.Features.Seafile.GetSeafileTrack;

public sealed record class GetSeafileTrackQuery(string Id) : IRequest<SeafileTrackDto?>;
