using Microsoft.Data.SqlClient;
using Migration.MigrationTools.Schema;

namespace Migration.MigrationTools.Providers;

public class SqlServerProvider : IDatabaseProvider
{
    private readonly string _connectionString;

    public SqlServerProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string ProviderName => "sqlserver";

    public async Task<List<TableSchema>> GetTablesAsync()
    {
        var query = @"
        SELECT 
            c.TABLE_SCHEMA,
            c.TABLE_NAME,
            c.COLUMN_NAME,
            c.DATA_TYPE,
            c.IS_NULLABLE,
            CASE WHEN pk.COL_NAME IS NOT NULL THEN 1 ELSE 0 END AS IS_PRIMARY_KEY,
            CASE WHEN c.COLUMN_DEFAULT IS NOT NULL AND c.COLUMN_DEFAULT LIKE '%newid%' THEN 1 ELSE 0 END AS IS_IDENTITY
        FROM INFORMATION_SCHEMA.COLUMNS c
        LEFT JOIN (
            SELECT 
                kc.TABLE_SCHEMA, 
                kc.TABLE_NAME, 
                ccu.COLUMN_NAME AS COL_NAME
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kc ON tc.CONSTRAINT_NAME = kc.CONSTRAINT_NAME
            JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE ccu ON kc.CONSTRAINT_NAME = ccu.CONSTRAINT_NAME
            WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
        ) pk ON c.TABLE_SCHEMA = pk.TABLE_SCHEMA 
            AND c.TABLE_NAME = pk.TABLE_NAME 
            AND c.COLUMN_NAME = pk.COL_NAME
        ORDER BY c.TABLE_SCHEMA, c.TABLE_NAME, c.ORDINAL_POSITION";

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();

        var rows = new List<(string Schema, string TableName, ColumnSchema Column)>();

        while (await reader.ReadAsync())
        {
            var column = new ColumnSchema
            {
                Name = reader.GetString(2),
                DataType = reader.GetString(3),
                IsNullable = reader.GetString(4) == "YES",
                IsPrimaryKey = reader.GetInt32(5) == 1,
                IsIdentity = reader.GetInt32(6) == 1
            };

            rows.Add((reader.GetString(0), reader.GetString(1), column));
        }

        // Grouping result by table
        return rows.GroupBy(r => new { r.Schema, r.TableName })
            .Select(g => new TableSchema
            {
                Name = $"{g.Key.Schema}.{g.Key.TableName}",
                Columns = g.Select(x => x.Column).ToList()
            })
            .OrderBy(t => t.Name)
            .ToList();
    }

    public string GetCreateTableScript(TableSchema table)
    {
        var columns = string.Join(",\n    ", table.Columns.Select(c =>
        {
            var nullable = c.IsNullable ? "NULL" : "NOT NULL";
            var identity = c.IsIdentity ? " IDENTITY(1,1)" : "";
            return $"    [{c.Name}] {c.DataType}{identity} {nullable}";
        }));

        var primaryKey = table.PrimaryKeys.Any()
            ? $",\n    CONSTRAINT [PK_{table.Name.Replace(".", "_")}] PRIMARY KEY ({string.Join(", ", table.PrimaryKeys.Select(k => $"[{k}]"))})"
            : "";

        return $"CREATE TABLE [{table.Name}] (\n{columns}{primaryKey}\n);";
    }

    public string GetDropTableScript(string tableName)
    {
        return $"DROP TABLE [{tableName}];";
    }

    public async Task CreateTableAsync(TableSchema table)
    {
        var script = GetCreateTableScript(table);
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DropTableAsync(string tableName)
    {
        var script = GetDropTableScript(tableName);
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        using var command = new SqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
    }
}
