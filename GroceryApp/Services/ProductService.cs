using MongoDB.Bson;
using MongoDB.Driver;
using GroceryApp.Models;
using GroceryApp.DTOs;

namespace GroceryApp.Services;

public class ProductService
{
    private readonly IMongoCollection<Product> _products;

    public ProductService(IConfiguration config)
    {
        var connectionString = config["MongoDB:ConnectionString"];
        var databaseName = config["MongoDB:DatabaseName"];
        if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("MongoDB connection string is missing.");
        if (string.IsNullOrWhiteSpace(databaseName)) throw new InvalidOperationException("MongoDB database name is missing.");

        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
        _products = database.GetCollection<Product>("Products");
    }

    // GET ALL PRODUCTS
    public async Task<List<Product>> GetAllAsync()
    {
        return await _products.Find(_ => true).ToListAsync();
    }

    // GET PRODUCT BY ID
    public async Task<Product?> GetByIdAsync(string id)
    {
        return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    // CREATE PRODUCT
    public async Task<Product> CreateAsync(ProductDto productDto)
    {
        var product = new Product
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = productDto.Name.Trim(),
            Category = productDto.Category.Trim(),
            Price = productDto.Price,
            Stock = productDto.Stock
        };

        await _products.InsertOneAsync(product);
        return product;
    }

    // UPDATE PRODUCT
    public async Task<bool> UpdateAsync(
        string id,
        ProductDto productDto)
    {
        var update = Builders<Product>.Update
            .Set(p => p.Name, productDto.Name.Trim())
            .Set(p => p.Category, productDto.Category.Trim())
            .Set(p => p.Price, productDto.Price)
            .Set(p => p.Stock, productDto.Stock);

        var result = await _products.UpdateOneAsync(p => p.Id == id, update);
        return result.MatchedCount > 0;
    }

    // DELETE PRODUCT
    public async Task<bool> DeleteAsync(string id)
    {
        var result = await _products.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }
}