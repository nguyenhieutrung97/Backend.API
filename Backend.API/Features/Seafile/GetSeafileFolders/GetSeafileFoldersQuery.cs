using MediatR;

namespace Backend.API.Features.Seafile.GetSeafileFolders;

public sealed record class GetSeafileFoldersQuery : IRequest<List<string>>;
