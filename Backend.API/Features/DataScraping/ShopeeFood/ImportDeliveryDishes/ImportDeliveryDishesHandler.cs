using Backend.API.Features.DataScraping.ShopeeFood.Models;
using Backend.API.Features.DataScraping.ShopeeFood.Store;
using MediatR;

namespace Backend.API.Features.DataScraping.ShopeeFood.ImportDeliveryDishes
{
    public sealed class ImportDeliveryDishesHandler : IRequestHandler<ImportDeliveryDishesCommand>
    {
        private readonly IDeliveryDishesMongoStore _store;
        public ImportDeliveryDishesHandler(IDeliveryDishesMongoStore store)
        {
            _store = store;
        }
        public async Task Handle(ImportDeliveryDishesCommand request, CancellationToken cancellationToken)
        {
            await _store.InsertAsync(request.DeliveryDishes, cancellationToken);
        }
    }
}
