namespace Backend.API.Features.Seafile.Models;

public sealed record class SeafileTrackFilter
{
    public string? Title { get; init; }
    public string? Src { get; init; }
    public string? Folder { get; init; }
    public DateTime? CreatedFrom { get; init; }
    public DateTime? CreatedTo { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
