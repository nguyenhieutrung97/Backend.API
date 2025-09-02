using Backend.API.Features.DataScraping.ShopeeFood.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.API.Features.DataScraping.ShopeeFood.Store
{
    public class DeliveryDishesMongoStore : IDeliveryDishesMongoStore
    {
        private readonly IMongoCollection<DeliveryDishes> _collection;

        public DeliveryDishesMongoStore(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDb:ConnectionString"] ?? "mongodb://localhost:27017";
            var databaseName = configuration["MongoDb:Database"] ?? "ShopeeFoodDb";
            var collectionName = configuration["MongoDb:DeliveryDishesCollection"] ?? "DeliveryDishes";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<DeliveryDishes>(collectionName);
        }

        public async Task InsertAsync(DeliveryDishes deliveryDishes, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(deliveryDishes, cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<DeliveryDishes>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task<DeliveryDishes?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<DeliveryDishes>.Filter.Eq("_id", ObjectId.Parse(id));
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<DeliveryDishes>.Filter.Eq("_id", ObjectId.Parse(id));
            var result = await _collection.DeleteOneAsync(filter, cancellationToken);
            return result.DeletedCount > 0;
        }
    }
}
