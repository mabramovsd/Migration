using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Migration.MigrationTools.Configuration;
using Migration.MigrationTools.Providers;

var builder = Host.CreateDefaultBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var settings = new AppSettings();
configuration.GetSection("AppSettings").Bind(settings);

builder.ConfigureServices((context, services) =>
{
    // Register database providers
    services.AddKeyedSingleton<Func<IDatabaseProvider>>("source", () =>
    {
        return settings.SourceProvider.ToLowerInvariant() switch
        {
            "sqlserver" => new SqlServerProvider(settings.SourceConnectionString),
            "postgres" => new PostgresProvider(settings.SourceConnectionString),
            _ => throw new InvalidOperationException($"Unknown provider: {settings.SourceProvider}")
        };
    });

    services.AddKeyedSingleton<Func<IDatabaseProvider>>("target", () =>
    {
        return settings.TargetProvider.ToLowerInvariant() switch
        {
            "sqlserver" => new SqlServerProvider(settings.TargetConnectionString),
            "postgres" => new PostgresProvider(settings.TargetConnectionString),
            _ => throw new InvalidOperationException($"Unknown provider: {settings.TargetProvider}")
        };
    });
});



var app = builder.Build();

Func<IDatabaseProvider> sourceFactory = app.Services.GetRequiredKeyedService<Func<IDatabaseProvider>>("source");
Func<IDatabaseProvider> targetFactory = app.Services.GetRequiredKeyedService<Func<IDatabaseProvider>>("target");

IDatabaseProvider sourceProvider = sourceFactory();
IDatabaseProvider targetProvider = targetFactory();
Console.WriteLine($"Source Provider: {sourceProvider.ProviderName}");
Console.WriteLine($"Target Provider: {targetProvider.ProviderName}");
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

    targetProvider.CreateTableAsync(table);
}

await app.RunAsync();
