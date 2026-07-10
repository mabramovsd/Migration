using Migration.MigrationTools.Schema;

namespace Migration.MigrationTools.Providers;

public interface ISchemaProvider
{
    Task<List<TableSchema>> GetTablesAsync();
}
