using Backend.API.Features.DataScraping.ShopeeFood.GetDeliveryDishes;
using Backend.API.Features.DataScraping.ShopeeFood.ImportDeliveryDishes;
using Backend.API.Features.DataScraping.ShopeeFood.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace Backend.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataScrapingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DataScrapingController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("import-delivery-dishes")]
        public async Task<IActionResult> ImportDeliveryDishes(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            DeliveryDishes? deliveryDishes;
            using (var stream = file.OpenReadStream())
            {
                try
                {
                    deliveryDishes = await JsonSerializer.DeserializeAsync<DeliveryDishes>(stream, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            if (deliveryDishes == null)
            {
                return BadRequest("Failed to parse delivery dishes data.");
            }

            await _mediator.Send(new ImportDeliveryDishesCommand(deliveryDishes));

            return Ok(new { message = "Data imported successfully." });
        }

        // GET /api/datascraping/delivery-dishes
        [HttpGet("delivery-dishes")]
        public async Task<IActionResult> GetDeliveryDishes()
        {
            var dishes = await _mediator.Send(new GetDeliveryDishesQuery());
            return Ok(dishes);
        }

        // GET /api/datascraping/delivery-dishes-flatten
        [HttpGet("delivery-dishes-flatten")]
        public async Task<IActionResult> GetDeliveryDishesFlatten()
        {
            var roots = await _mediator.Send(new GetDeliveryDishesQuery());
            if (roots == null || roots.Count == 0)
            {
                return Ok(Array.Empty<DeliveryDishesFlatten>());
            }

            var rows = new List<DeliveryDishesFlatten>();

            foreach (var root in roots)
            {
                if (root?.Reply?.MenuInfos == null) continue;

                foreach (var menu in root.Reply.MenuInfos)
                {
                    var dishTypeName = menu?.DishTypeName ?? "";
                    if (menu?.Dishes == null) continue;

                    foreach (var dish in menu.Dishes)
                    {
                        var baseRow = new DeliveryDishesFlatten
                        {
                            DeliveryDishesId = root.Id ?? "",
                            DeliveryDishesName = root.Name ?? "",
                            DishTypeName = dishTypeName,
                            DishName = dish?.Name ?? "",
                            DishDescription = dish?.Description ?? "",
                            DishPriceText = dish?.Price?.Text ?? "",
                            DishPhotoUrl = dish?.Photos?.FirstOrDefault()?.Value
                        };

                        // 1) Always include the base dish row (without option context)
                        rows.Add(baseRow);

                        // 2) Add a row for each option item (if any)
                        if (dish?.Options == null) continue;

                        foreach (var opt in dish.Options)
                        {
                            if (opt?.OptionItems?.Items == null || opt.OptionItems.Items.Count == 0) continue;

                            foreach (var item in opt.OptionItems.Items)
                            {
                                rows.Add(baseRow with
                                {
                                    OptionName = opt.Name,
                                    OptionItemName = item?.Name,
                                    OptionItemPriceText = item?.Price?.Text
                                });
                            }
                        }
                    }
                }
            }

            return Ok(rows);
        }

        // GET /api/datascraping/delivery-dishes-flatten-csv
        [HttpGet("delivery-dishes-flatten-csv")]
        public async Task<IActionResult> GetDeliveryDishesFlattenCsv()
        {
            var roots = await _mediator.Send(new GetDeliveryDishesQuery());
            if (roots == null || roots.Count == 0)
            {
                return File(Encoding.UTF8.GetBytes(BuildCsv(Array.Empty<DeliveryDishesFlatten>())),
                    "text/csv", "delivery-dishes-flatten.csv");
            }

            var rows = new List<DeliveryDishesFlatten>();

            foreach (var root in roots)
            {
                if (root?.Reply?.MenuInfos == null) continue;

                foreach (var menu in root.Reply.MenuInfos)
                {
                    var dishTypeName = menu?.DishTypeName ?? "";
                    if (menu?.Dishes == null) continue;

                    foreach (var dish in menu.Dishes)
                    {
                        var baseRow = new DeliveryDishesFlatten
                        {
                            DeliveryDishesId = root.Id ?? "",
                            DeliveryDishesName = root.Name ?? "",
                            DishTypeName = dishTypeName,
                            DishName = dish?.Name ?? "",
                            DishDescription = dish?.Description ?? "",
                            DishPriceText = dish?.Price?.Text ?? "",
                            DishPhotoUrl = dish?.Photos?.FirstOrDefault()?.Value
                        };

                        rows.Add(baseRow);

                        if (dish?.Options == null) continue;

                        foreach (var opt in dish.Options)
                        {
                            if (opt?.OptionItems?.Items == null || opt.OptionItems.Items.Count == 0) continue;

                            foreach (var item in opt.OptionItems.Items)
                            {
                                rows.Add(baseRow with
                                {
                                    OptionName = opt.Name,
                                    OptionItemName = item?.Name,
                                    OptionItemPriceText = item?.Price?.Text
                                });
                            }
                        }
                    }
                }
            }

            string csv = BuildCsv(rows);
            return File(Encoding.UTF8.GetBytes(csv), "text/csv", "delivery-dishes-flatten.csv");
        }

        private static string BuildCsv(IEnumerable<DeliveryDishesFlatten> rows)
        {
            var sb = new StringBuilder();

            string[] headers = new[]
            {
                "DeliveryDishesId",
                "DeliveryDishesName",
                "DishTypeName",
                "DishName",
                "DishDescription",
                "DishPriceText",
                "DishPhotoUrl",
                "OptionName",
                "OptionItemName",
                "OptionItemPriceText"
            };
            sb.AppendLine(string.Join(",", headers));

            foreach (var r in rows)
            {
                sb.AppendLine(string.Join(",", new[]
                {
                    Csv(r.DeliveryDishesId),
                    Csv(r.DeliveryDishesName),
                    Csv(r.DishTypeName),
                    Csv(r.DishName),
                    Csv(r.DishDescription),
                    Csv(r.DishPriceText),
                    Csv(r.DishPhotoUrl),
                    Csv(r.OptionName),
                    Csv(r.OptionItemName),
                    Csv(r.OptionItemPriceText)
                }));
            }

            return sb.ToString();
        }

        private static string Csv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            // escape double quotes by replacing " with ""
            value = value.Replace("\"", "\"\"");
            return $"\"{value}\"";
        }
    }
}
