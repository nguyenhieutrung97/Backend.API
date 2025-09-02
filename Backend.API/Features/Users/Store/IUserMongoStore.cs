using Backend.API.Features.Users.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend.API.Features.Users.Store
{
    public interface IUserMongoStore
    {
        Task<User> InsertAsync(User user, CancellationToken cancellationToken = default);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(Guid id, User user, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
