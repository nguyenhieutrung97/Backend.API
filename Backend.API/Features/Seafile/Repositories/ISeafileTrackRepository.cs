using Backend.API.Common;
using Backend.API.Features.Seafile.Models;

namespace Backend.API.Features.Seafile.Repositories;

public interface ISeafileTrackRepository
{
    Task<SeafileTrack?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<Paged<SeafileTrack>> GetPagedAsync(SeafileTrackFilter filter, CancellationToken cancellationToken = default);
    Task<SeafileTrack> CreateAsync(SeafileTrack track, CancellationToken cancellationToken = default);
    Task<SeafileTrack> UpdateAsync(SeafileTrack track, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task<List<string>> GetDistinctFoldersAsync(CancellationToken cancellationToken = default);
}
