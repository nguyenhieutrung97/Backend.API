using Backend.API.Features.DataScraping.ShopeeFood.Models;
using Backend.API.Features.DataScraping.ShopeeFood.Store;
using MediatR;

namespace Backend.API.Features.DataScraping.ShopeeFood.GetDeliveryDishes
{
    public sealed record GetDeliveryDishesQuery() : IRequest<IReadOnlyList<DeliveryDishes>>;

    public sealed class GetDeliveryDishesHandler : IRequestHandler<GetDeliveryDishesQuery, IReadOnlyList<DeliveryDishes>>
    {
        private readonly IDeliveryDishesMongoStore _store;
        public GetDeliveryDishesHandler(IDeliveryDishesMongoStore store)
        {
            _store = store;
        }
        public async Task<IReadOnlyList<DeliveryDishes>> Handle(GetDeliveryDishesQuery request, CancellationToken cancellationToken)
        {
            return await _store.GetAllAsync(cancellationToken);
        }
    }
}
