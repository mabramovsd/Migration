using Migration.MigrationTools.Schema;

namespace Migration.MigrationTools.Providers;

public interface IDatabaseProvider : ISchemaProvider, ITableCreator
{
    string ProviderName { get; }
    string GetCreateTableScript(TableSchema table);
    string GetDropTableScript(string tableName);
}
