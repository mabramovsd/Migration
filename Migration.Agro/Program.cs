using Microsoft.EntityFrameworkCore;
using Migration.Agro;
using Migration.Agro.Middlewares;
using Migration.Agro.Services;
using Migration.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Connection strings
var agroCs = builder.Configuration.GetConnectionString("AgroDb");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICompanyService, HRServiceAgro>();

// DB context
builder.Services.AddDbContext<AgroDBContext>(options =>
    options.UseSqlServer(agroCs));

var app = builder.Build();

// Apply migrations automatically (for development only)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var agroContext = scope.ServiceProvider.GetRequiredService<AgroDBContext>();
        if (agroContext.Database.GetPendingMigrations().Any())
        {
            agroContext.Database.Migrate();
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
