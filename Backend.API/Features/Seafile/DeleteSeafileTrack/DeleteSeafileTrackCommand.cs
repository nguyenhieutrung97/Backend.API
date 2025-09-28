using MediatR;

namespace Backend.API.Features.Seafile.DeleteSeafileTrack;

public sealed record class DeleteSeafileTrackCommand(string Id) : IRequest<bool>;
