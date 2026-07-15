using Microsoft.EntityFrameworkCore;
using Migration.Contracts;
using MigrationWeb;
using MigrationWeb.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Connection strings
var coreCs = builder.Configuration.GetConnectionString("CoreDb");

// Add services to the container.
var controllers = builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JSON serialization to not escape Unicode (for Cyrillic)
controllers.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});

#region Company Services
// Configure ServiceUrls
var serviceUrlsSection = builder.Configuration.GetSection("ServiceUrls");
var serviceUrls = new ServiceUrls();
serviceUrlsSection.Bind(serviceUrls);
builder.Services.Configure<ServiceUrls>(serviceUrlsSection);

// Register HttpClient for company services
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

// keyed registration for company services
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
#endregion Company Services

builder.Services.AddScoped<HRService>();

// DB context
builder.Services.AddDbContext<CoreDBContext>(options =>
    options.UseSqlServer(coreCs));

var app = builder.Build();

// Apply migrations automatically
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

// Serve static files (SPA)
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

// Default route for SPA
app.MapFallbackToFile("index.html");

app.MapControllers();

app.Run();
