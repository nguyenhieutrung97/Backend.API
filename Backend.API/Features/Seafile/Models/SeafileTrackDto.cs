namespace Backend.API.Features.Seafile.Models;

public sealed record class SeafileTrackDto
{
    public string Id { get; init; } = default!;
    public string Title { get; init; } = default!;
    public string Src { get; init; } = default!;
    public string Folder { get; init; } = default!;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
