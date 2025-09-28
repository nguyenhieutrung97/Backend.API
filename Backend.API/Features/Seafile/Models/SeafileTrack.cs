using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.API.Features.Seafile.Models;

public sealed record class SeafileTrack
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = default!;
    
    public string Title { get; init; } = default!;
    
    public string Src { get; init; } = default!;
    
    public string Folder { get; init; } = default!;
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
}
