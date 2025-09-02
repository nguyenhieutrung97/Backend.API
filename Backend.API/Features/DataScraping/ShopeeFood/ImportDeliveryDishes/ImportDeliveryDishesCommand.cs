using Backend.API.Features.DataScraping.ShopeeFood.Models;
using MediatR;

namespace Backend.API.Features.DataScraping.ShopeeFood.ImportDeliveryDishes
{
    public sealed record ImportDeliveryDishesCommand(DeliveryDishes DeliveryDishes) : IRequest;
}
