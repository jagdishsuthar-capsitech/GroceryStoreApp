using GroceryApp.Data;
using GroceryApp.Models;
using GroceryApp.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// MongoDB Configuration
var mongoSettings = builder.Configuration
    .GetSection("MongoDB")
    .Get<MongoDbSettings>() ?? throw new InvalidOperationException("MongoDB configuration is missing.");

if (string.IsNullOrWhiteSpace(mongoSettings.ConnectionString))
    throw new InvalidOperationException("MongoDB connection string is missing.");

if (string.IsNullOrWhiteSpace(mongoSettings.DatabaseName))
    throw new InvalidOperationException("MongoDB database name is missing.");

// MongoDB Client
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(mongoSettings.ConnectionString));

// MongoDB Database
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoSettings.DatabaseName);
});

// Product Collection
builder.Services.AddSingleton<IMongoCollection<Product>>(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<Product>("Products");
});

// Store Collection
builder.Services.AddSingleton<IMongoCollection<Store>>(sp =>
{
    var database = sp.GetRequiredService<IMongoDatabase>();
    return database.GetCollection<Store>("Stores");
});

// Application Services
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<StoreService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173","http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Development Environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
// app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Root Endpoint
app.MapGet("/", () => "My API Server is Running!");

app.Run();