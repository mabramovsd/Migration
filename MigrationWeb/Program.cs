using Microsoft.EntityFrameworkCore;
using Migration.Agro;
using Migration.Agro.Services;
using Migration.Contracts;
using MigrationWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();





var connectionStringAgro = "Data Source=MSI;Initial Catalog=Migration_Agro;Integrated Security=True;Trust Server Certificate=True";
builder.Services.AddDbContext<AgroDBContext>(options =>
{
    options.UseSqlServer(connectionStringAgro);
});

builder.Services.AddScoped<HRServiceAgro>();




//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionString = "Data Source=MSI;Initial Catalog=Migration_Core;Integrated Security=True;Trust Server Certificate=True";
builder.Services.AddDbContext<CoreDBContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<HRService>();



var app = builder.Build();

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
