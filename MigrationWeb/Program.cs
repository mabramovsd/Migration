using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Migration.Contracts;
using MigrationWeb;
using MigrationWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Connection strings
builder.Services.AddOptions();
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
builder.Services.AddHttpClient("Agro", client =>
{
    var urls = builder.Configuration.GetSection("ServiceUrls").Get<ServiceUrls>();
    client.BaseAddress = new Uri(urls?.Agro ?? "http://localhost:5002");
});

builder.Services.AddHttpClient("Shipbuilding", client =>
{
    var urls = builder.Configuration.GetSection("ServiceUrls").Get<ServiceUrls>();
    client.BaseAddress = new Uri(urls?.Shipbuilding ?? "http://localhost:5001");
});

builder.Services.AddKeyedScoped<ICompanyService>("Agro", (sp, key) =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Agro");
    var logger = sp.GetRequiredService<ILogger<HTTPCompanyService>>();
    return new HTTPCompanyService(httpClient, logger);
});

builder.Services.AddKeyedScoped<ICompanyService>("Shipbuilding", (sp, key) =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Shipbuilding");
    var logger = sp.GetRequiredService<ILogger<HTTPCompanyService>>();
    return new HTTPCompanyService(httpClient, logger);
});

// keyed registration for ICompanyService
builder.Services.AddKeyedScoped<ICompanyService>("AgroHttp", (sp, key) =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Agro");
    var logger = sp.GetRequiredService<ILogger<HTTPCompanyService>>();
    return new HTTPCompanyService(httpClient, logger);
});

builder.Services.AddKeyedScoped<ICompanyService>("ShipbuildingHttp", (sp, key) =>
{
    var httpClient = sp.GetRequiredService<IHttpClientFactory>().CreateClient("Shipbuilding");
    var logger = sp.GetRequiredService<ILogger<HTTPCompanyService>>();
    return new HTTPCompanyService(httpClient, logger);
});

builder.Services.AddScoped<HRService>();

// DB context
builder.Services.AddDbContext<CoreDBContext>(options =>
    options.UseSqlServer(coreCs));

var app = builder.Build();

// Apply migrations automatically (for development only)
if (app.Environment.IsDevelopment())
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var coreContext = scope.ServiceProvider.GetRequiredService<CoreDBContext>();
            if (coreContext.Database.GetPendingMigrations().Any())
            {
                coreContext.Database.Migrate();
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
