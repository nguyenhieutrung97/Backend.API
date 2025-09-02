using Backend.API.Features.Users.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Backend.API.Features.Users.Store
{
    public class UserMongoStore : IUserMongoStore
    {
        private readonly IMongoCollection<User> _collection;

        public UserMongoStore(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDb:ConnectionString"] ?? "mongodb://localhost:27017";
            var databaseName = configuration["MongoDb:Database"] ?? "ShopeeFoodDb";
            var collectionName = "Users";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<User>(collectionName);
        }

        public async Task<User> InsertAsync(User user, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(user, cancellationToken: cancellationToken);
            return user;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _collection.Find(_ => true).ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Guid id, User user, CancellationToken cancellationToken = default)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var result = await _collection.ReplaceOneAsync(filter, user, cancellationToken: cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var result = await _collection.DeleteOneAsync(filter, cancellationToken);
            return result.DeletedCount > 0;
        }
    }
}
