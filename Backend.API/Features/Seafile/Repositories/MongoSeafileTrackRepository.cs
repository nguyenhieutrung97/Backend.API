using Backend.API.Common;
using Backend.API.Features.Seafile.Models;
using MongoDB.Driver;

namespace Backend.API.Features.Seafile.Repositories;

public sealed class MongoSeafileTrackRepository : ISeafileTrackRepository
{
    private readonly IMongoCollection<SeafileTrack> _collection;

    public MongoSeafileTrackRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<SeafileTrack>("seafile_tracks");
    }

    public async Task<SeafileTrack?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Paged<SeafileTrack>> GetPagedAsync(SeafileTrackFilter filter, CancellationToken cancellationToken = default)
    {
        var filterBuilder = Builders<SeafileTrack>.Filter;
        var filters = new List<FilterDefinition<SeafileTrack>>();

        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            filters.Add(filterBuilder.Regex(x => x.Title, new MongoDB.Bson.BsonRegularExpression(filter.Title, "i")));
        }

        if (!string.IsNullOrWhiteSpace(filter.Src))
        {
            filters.Add(filterBuilder.Regex(x => x.Src, new MongoDB.Bson.BsonRegularExpression(filter.Src, "i")));
        }

        if (!string.IsNullOrWhiteSpace(filter.Folder))
        {
            filters.Add(filterBuilder.Regex(x => x.Folder, new MongoDB.Bson.BsonRegularExpression(filter.Folder, "i")));
        }

        if (filter.CreatedFrom.HasValue)
        {
            filters.Add(filterBuilder.Gte(x => x.CreatedAt, filter.CreatedFrom.Value));
        }

        if (filter.CreatedTo.HasValue)
        {
            filters.Add(filterBuilder.Lte(x => x.CreatedAt, filter.CreatedTo.Value));
        }

        var combinedFilter = filters.Any() ? filterBuilder.And(filters) : filterBuilder.Empty;

        var totalCount = await _collection.CountDocumentsAsync(combinedFilter, cancellationToken: cancellationToken);

        var skip = (filter.Page - 1) * filter.PageSize;
        var items = await _collection
            .Find(combinedFilter)
            .SortByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Limit(filter.PageSize)
            .ToListAsync(cancellationToken);

        return new Paged<SeafileTrack>(items, filter.Page, filter.PageSize, (int)totalCount);
    }

    public async Task<SeafileTrack> CreateAsync(SeafileTrack track, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(track, cancellationToken: cancellationToken);
        return track;
    }

    public async Task<SeafileTrack> UpdateAsync(SeafileTrack track, CancellationToken cancellationToken = default)
    {
        var updatedTrack = track with { UpdatedAt = DateTime.UtcNow };
        await _collection.ReplaceOneAsync(x => x.Id == track.Id, updatedTrack, cancellationToken: cancellationToken);
        return updatedTrack;
    }

    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var result = await _collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
        return result.DeletedCount > 0;
    }

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var count = await _collection.CountDocumentsAsync(x => x.Id == id, cancellationToken: cancellationToken);
        return count > 0;
    }

    public async Task<List<string>> GetDistinctFoldersAsync(CancellationToken cancellationToken = default)
    {
        var folders = await _collection.DistinctAsync(x => x.Folder, FilterDefinition<SeafileTrack>.Empty, cancellationToken: cancellationToken);
        return await folders.ToListAsync(cancellationToken);
    }
}
