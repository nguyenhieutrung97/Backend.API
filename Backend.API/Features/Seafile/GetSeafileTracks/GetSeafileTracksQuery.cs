using Backend.API.Common;
using Backend.API.Features.Seafile.Models;
using MediatR;

namespace Backend.API.Features.Seafile.GetSeafileTracks;

public sealed record class GetSeafileTracksQuery(SeafileTrackFilter Filter) : IRequest<Paged<SeafileTrackDto>>;
