using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GroceryApp.Models;

public class Store
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("Location")]
    public string Location { get; set; } = string.Empty;

    [BsonElement("Inventory")]
    public List<Product> Inventory { get; set; } = new();
}