using Microsoft.EntityFrameworkCore;
using Migration.Contracts;
using Migration.Shipbuilding;
using Migration.Shipbuilding.Middlewares;
using Migration.Shipbuilding.Services;

var builder = WebApplication.CreateBuilder(args);

// Connection strings
var shipCs = builder.Configuration.GetConnectionString("ShipbuildingDb");
var serviceUrl = builder.Configuration.GetValue<string>("ServiceUrl");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register ICompanyService (only one implementation in Shipbuilding service)
builder.Services.AddScoped<ICompanyService, HRServiceShipbuilding>();

// Configure ServiceUrl for HTTP client
builder.Services.Configure<ServiceUrls>(builder.Configuration.GetSection("ServiceUrls"));

// DB context
builder.Services.AddDbContext<ShipbuildingDBContext>(options =>
    options.UseSqlServer(shipCs));

var app = builder.Build();

// Apply migrations automatically (for development only)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var shipContext = scope.ServiceProvider.GetRequiredService<ShipbuildingDBContext>();
        if (shipContext.Database.GetPendingMigrations().Any())
        {
            shipContext.Database.Migrate();
        }
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
