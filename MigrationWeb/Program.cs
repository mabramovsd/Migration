using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
var serviceUrlsSection = builder.Configuration.GetSection("ServiceUrls");
var serviceUrls = new ServiceUrls();
serviceUrlsSection.Bind(serviceUrls);
Console.WriteLine($"ServiceUrls loaded: Agro={serviceUrls.Agro}, Shipbuilding={serviceUrls.Shipbuilding}");
builder.Services.Configure<ServiceUrls>(serviceUrlsSection);

// Register HttpClient for Agro service as singleton
builder.Services.AddSingleton<HttpClient>(sp =>
{
    var serviceUrlsOptions = sp.GetRequiredService<IOptions<ServiceUrls>>().Value;
    var httpClient = new HttpClient(new HttpClientHandler())
    {
        BaseAddress = new Uri(serviceUrlsOptions.Agro ?? "http://localhost:5002")
    };
    return httpClient;
});

// keyed registration for ICompanyService
builder.Services.AddKeyedScoped<ICompanyService, HTTPCompanyService>("AgroHttp");

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
