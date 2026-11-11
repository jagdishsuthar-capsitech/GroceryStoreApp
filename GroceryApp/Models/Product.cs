using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GroceryApp.Models;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("Category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("Price")]
    public decimal Price { get; set; }

    [BsonElement("Stock")]
    public int Stock { get; set; }
}