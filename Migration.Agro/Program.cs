using Microsoft.EntityFrameworkCore;
using Migration.Agro;
using Migration.Agro.Services;
using Migration.Contracts.Interfaces;
using Migration.Contracts.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Connection strings
var agroCs = builder.Configuration.GetConnectionString("AgroDb");

// Add services to the container.
var controllers = builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCorrelationIdSupport();
builder.Services.AddScoped<ICompanyService, HRServiceAgro>();

// Configure JSON serialization to not escape Unicode (for Cyrillic)
controllers.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
});

// DB context
builder.Services.AddDbContext<AgroDBContext>(options =>
    options.UseSqlServer(agroCs));

var app = builder.Build();

// Apply migrations automatically (for development only)
if (app.Environment.IsDevelopment())
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AgroDBContext>();
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

app.UseCorrelationId();
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
