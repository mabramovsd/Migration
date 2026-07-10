using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Migration.MigrationTools.Configuration;
using Migration.MigrationTools.Providers;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    services.AddSingleton(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        return new AppSettings
        {
            SourceProvider = configuration["AppSettings:SourceProvider"] ?? "sqlserver",
            SourceConnectionString = configuration["AppSettings:SourceConnectionString"] ?? "Server=.;Database=Migration_Agro;Trusted_Connection=True;Trust Server Certificate=True",
            TargetProvider = configuration["AppSettings:TargetProvider"] ?? "postgres",
            TargetConnectionString = configuration["AppSettings:TargetConnectionString"] ?? "Host=localhost;Port=5432;Database=Migration_Agro_Postgres;Username=postgres;Password=postgres"
        };
    });

    // Register database providers
    services.AddSingleton<IDatabaseProvider>(provider =>
    {
        var settings = provider.GetRequiredService<AppSettings>();
        return settings.SourceProvider.ToLowerInvariant() switch
        {
            "sqlserver" => new SqlServerProvider(settings.SourceConnectionString),
            "postgres" => throw new NotImplementedException("Postgres provider not implemented yet"),
            _ => throw new InvalidOperationException($"Unknown provider: {settings.SourceProvider}")
        };
    });
});

var app = builder.Build();

var sourceProvider = app.Services.GetRequiredService<IDatabaseProvider>();

Console.WriteLine($"Source Provider: {sourceProvider.ProviderName}");
Console.WriteLine("Getting tables from source database...\n");

var tables = await sourceProvider.GetTablesAsync();

Console.WriteLine($"Found {tables.Count} tables:\n");
foreach (var table in tables)
{
    Console.WriteLine($"  - {table.Name} ({table.Columns.Count} columns)");
    foreach (var column in table.Columns)
    {
        Console.WriteLine($"      {column.Name}: {column.DataType} {(column.IsNullable ? "NULL" : "NOT NULL")} {(column.IsPrimaryKey ? "[PK]" : "")} {(column.IsIdentity ? "[IDENTITY]" : "")}");
    }
}

await app.RunAsync();
