using Microsoft.EntityFrameworkCore;
using Migration.Contracts;
using Migration.Shipbuilding;
using Migration.Shipbuilding.Middlewares;
using Migration.Shipbuilding.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Connection strings
var shipCs = builder.Configuration.GetConnectionString("ShipDb");

// Add services to the container.
var controllers = builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICompanyService, HRServiceShipbuilding>();

// Configure JSON serialization to not escape Unicode (for Cyrillic)
controllers.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});

// DB context
builder.Services.AddDbContext<ShipbuildingDBContext>(options =>
    options.UseSqlServer(shipCs));

var app = builder.Build();

// Apply migrations automatically (for development only)
if (app.Environment.IsDevelopment())
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ShipbuildingDBContext>();
            if (dbContext.Database.GetPendingMigrations().Any())
            {
                dbContext.Database.Migrate();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to apply migrations: {ex.Message}");
    }
}

app.UseErrorHandling();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
