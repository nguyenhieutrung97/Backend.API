using Backend.API.Features.DataScraping.ShopeeFood.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Backend.API.Features.DataScraping.ShopeeFood.Store
{
    public interface IDeliveryDishesMongoStore
    {
        Task InsertAsync(DeliveryDishes deliveryDishes, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<DeliveryDishes>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<DeliveryDishes?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
