using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Backend.API.Features.DataScraping.ShopeeFood.Models
{
    public class DeliveryDishes
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string Name { get; set; } = "";
        public required Reply Reply { get; set; }
    }

    public class Reply
    {
        [JsonPropertyName("menu_infos")]
        public required List<MenuInfo> MenuInfos { get; set; }
    }

    public class MenuInfo
    {
        [JsonPropertyName("dish_type_name")]
        public string DishTypeName { get; set; } = "";

        [JsonPropertyName("dishes")]
        public List<Dish> Dishes { get; set; } = new();
    }

    public class Dish
    {
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("description")] public string Description { get; set; } = "";
        [JsonPropertyName("price")] public required Price Price { get; set; }
        [JsonPropertyName("photos")] public List<Photo>? Photos { get; set; }
        [JsonPropertyName("options")] public List<Option>? Options { get; set; }
    }

    public class Price
    {
        [JsonPropertyName("text")] public string Text { get; set; } = "";
    }

    public class Photo
    {
        [JsonPropertyName("value")] public string Value { get; set; } = "";
    }

    public class Option
    {
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("option_items")] public required OptionItems OptionItems { get; set; }
    }

    public class OptionItems
    {
        [JsonPropertyName("items")] public List<OptionItem> Items { get; set; } = new();
    }

    public class OptionItem
    {
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("price")] public required Price Price { get; set; }
    }
}
