using Microsoft.EntityFrameworkCore;
using Migration.Agro;
using Migration.Agro.Services;
using Migration.Contracts;
using Migration.Shipbuilding;
using Migration.Shipbuilding.Services;
using MigrationWeb;
using MigrationWeb.Services;

var builder = WebApplication.CreateBuilder(args);

//Connection strings
builder.Services.AddOptions();
var agroCs = builder.Configuration.GetConnectionString("AgroDb");
var shipCs = builder.Configuration.GetConnectionString("ShipbuildingDb");
var coreCs = builder.Configuration.GetConnectionString("CoreDb");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// When we will execute services separately
//builder.Services.Configure<ServiceUrls>(builder.Configuration.GetSection("ServiceUrls"));

// keyed services for ICompanyServices
builder.Services.AddKeyedScoped<ICompanyService, HRServiceAgro>("Agro");
builder.Services.AddKeyedScoped<ICompanyService, HRServiceShipbuilding>("Shipbuilding");
builder.Services.AddScoped<HRService>();

//DB contexts
builder.Services.AddDbContext<AgroDBContext>(options =>
    options.UseSqlServer(agroCs));

builder.Services.AddDbContext<ShipbuildingDBContext>(options =>
    options.UseSqlServer(shipCs));

builder.Services.AddDbContext<CoreDBContext>(options =>
    options.UseSqlServer(coreCs));




var app = builder.Build();

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
