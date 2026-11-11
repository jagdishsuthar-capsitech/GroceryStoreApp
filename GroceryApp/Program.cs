using GroceryApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Dependency Injection
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<StoreService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Authorization services
builder.Services.AddAuthorization();

var app = builder.Build();

// Development configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware
// app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthorization();

// Controllers
app.MapControllers();

// Default endpoint
app.MapGet("/", () =>
    "Grocery API Server is Running!");

app.Run();