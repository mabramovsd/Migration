using Microsoft.EntityFrameworkCore;
using Migration.Agro.Services;
using Migration.Contracts;
using Migration.Shipbuilding;
using Migration.Shipbuilding.Services;
using MigrationWeb;
using MigrationWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Connection strings
builder.Services.AddOptions();
var shipCs = builder.Configuration.GetConnectionString("ShipbuildingDb");
var coreCs = builder.Configuration.GetConnectionString("CoreDb");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure ServiceUrls
builder.Services.Configure<ServiceUrls>(builder.Configuration.GetSection("ServiceUrls"));

// HTTP services (running as separate microservices)
builder.Services.AddKeyedScoped<ICompanyService, HTTPCompanyService>("AgroHttp");
builder.Services.AddKeyedScoped<ICompanyService, HTTPCompanyService>("ShipbuildingHttp");

// Register HTTP clients
builder.Services.AddHttpClient<HTTPCompanyService>("AgroHttpClient", options =>
{
    var serviceUrls = builder.Configuration.GetSection("ServiceUrls").Get<ServiceUrls>();
    options.BaseAddress = new Uri(serviceUrls?.Agro ?? "http://localhost:5002");
});

builder.Services.AddHttpClient<HTTPCompanyService>("ShipbuildingHttpClient", options =>
{
    var serviceUrls = builder.Configuration.GetSection("ServiceUrls").Get<ServiceUrls>();
    options.BaseAddress = new Uri(serviceUrls?.Shipbuilding ?? "http://localhost:5001");
});

builder.Services.AddKeyedScoped<ICompanyService, HRServiceShipbuilding>("Shipbuilding");
builder.Services.AddScoped<HRService>();

// DB context (only CoreDBContext in MigrationWeb now)
builder.Services.AddDbContext<ShipbuildingDBContext>(options =>
    options.UseSqlServer(shipCs));

builder.Services.AddDbContext<CoreDBContext>(options =>
    options.UseSqlServer(coreCs));

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
        shipContext.Database.Migrate();
        
        var coreContext = scope.ServiceProvider.GetRequiredService<CoreDBContext>();
        if (coreContext.Database.GetPendingMigrations().Any())
        {
            coreContext.Database.Migrate();
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
