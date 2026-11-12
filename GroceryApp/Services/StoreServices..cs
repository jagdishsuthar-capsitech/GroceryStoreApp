using MongoDB.Bson;
using MongoDB.Driver;
using GroceryApp.Models;
using GroceryApp.DTOs;

namespace GroceryApp.Services;

public class StoreService
{
    private readonly IMongoCollection<Store> _stores;

    public StoreService(IMongoCollection<Store> stores)
    {
        _stores = stores;
    }

    // GET ALL STORES WITH PAGINATION
    public async Task<PagedApiResponse<Store>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var filter = Builders<Store>.Filter.Empty;
        var totalRecords = await _stores.CountDocumentsAsync(filter);
        var stores = await _stores
            .Find(filter)
            .SortBy(s => s.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

        return new PagedApiResponse<Store>
        {
            Items = stores,
            TotalRecords = (int)totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
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