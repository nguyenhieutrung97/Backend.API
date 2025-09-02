namespace Backend.API.Features.DataScraping.ShopeeFood.Models
{
    public sealed record class DeliveryDishesFlatten
    {
        public string DeliveryDishesId { get; init; } = "";
        public string DeliveryDishesName { get; init; } = "";

        public string DishTypeName { get; init; } = "";
        public string DishName { get; init; } = "";
        public string DishDescription { get; init; } = "";
        public string DishPriceText { get; init; } = "";
        public string? DishPhotoUrl { get; init; }

        // Option context (nullable when representing the base dish row)
        public string? OptionName { get; init; }
        public string? OptionItemName { get; init; }
        public string? OptionItemPriceText { get; init; }
    }
}