using Migration.MigrationTools.Schema;

namespace Migration.MigrationTools.Providers;

public interface ITableCreator
{
    Task CreateTableAsync(TableSchema table);
    Task DropTableAsync(string tableName);
}
