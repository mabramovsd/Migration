using Migration.MigrationTools.Schema;

namespace Migration.MigrationTools.Providers;

public interface ISchemaProvider
{
    Task<List<TableSchema>> GetTablesAsync();
    Task<TableSchema?> GetTableAsync(string tableName);
}
