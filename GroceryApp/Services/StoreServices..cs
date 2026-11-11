using MongoDB.Bson;
using MongoDB.Driver;
using GroceryApp.Models;
using GroceryApp.DTOs;

namespace GroceryApp.Services;

public class StoreService
{
    private readonly IMongoCollection<Store> _stores;

    public StoreService(IConfiguration config)
    {
        var connectionString = config["MongoDB:ConnectionString"];
        var databaseName = config["MongoDB:DatabaseName"];
        if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("MongoDB connection string is missing.");
        if (string.IsNullOrWhiteSpace(databaseName)) throw new InvalidOperationException("MongoDB database name is missing.");

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);

        // Keeping your original collection name "Store"
        _stores = database.GetCollection<Store>("Store");
    }

    // GET ALL STORES
    public async Task<List<Store>> GetAllAsync()
    {
        return await _stores.Find(_ => true).ToListAsync();
    }

    // GET STORE BY ID
    public async Task<Store?> GetByIdAsync(string id)
    {
        return await _stores.Find(s => s.Id == id).FirstOrDefaultAsync();
    }

    // CREATE STORE
    public async Task<Store> CreateAsync(StoreDto storeDto)
    {
        var store = new Store
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = storeDto.Name.Trim(),
            Location = storeDto.Location.Trim(),
            Inventory = storeDto.Inventory.Select(p => new Product
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = p.Name.Trim(),
                Category = p.Category.Trim(),
                Price = p.Price,
                Stock = p.Stock
            }).ToList()
        };

        await _stores.InsertOneAsync(store);
        return store;
    }

    // UPDATE STORE
    public async Task<bool> UpdateAsync(string id, StoreDto storeDto)
    {
        var existingStore = await GetByIdAsync(id);
        if (existingStore == null)
            return false;

        var inventory = storeDto.Inventory.Select(p =>
        {
            // Preserve ID if this product already belongs
            // to the existing store.
            var existingProduct = existingStore.Inventory.FirstOrDefault(x => x.Id == p.Id);

            return new Product
            {
                Id = existingProduct?.Id ?? ObjectId.GenerateNewId().ToString(),
                Name = p.Name.Trim(),
                Category = p.Category.Trim(),
                Price = p.Price,
                Stock = p.Stock
            };
        }).ToList();

        var update = Builders<Store>.Update
            .Set(s => s.Name, storeDto.Name.Trim())
            .Set(s => s.Location, storeDto.Location.Trim())
            .Set(s => s.Inventory, inventory);

        var result = await _stores.UpdateOneAsync(s => s.Id == id, update);
        return result.MatchedCount > 0;
    }

    // DELETE STORE
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _stores.DeleteOneAsync(s => s.Id == id);
        return result.DeletedCount > 0;
    }
}