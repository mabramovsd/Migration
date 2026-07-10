using Migration.MigrationTools.Schema;
using Npgsql;

namespace Migration.MigrationTools.Providers;

public class PostgresProvider : IDatabaseProvider
{
    private readonly string _connectionString;

    public PostgresProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string ProviderName => "postgres";

    public async Task<List<TableSchema>> GetTablesAsync()
    {
        var tables = new List<TableSchema>();

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT 
                c.table_schema,
                c.table_name,
                c.column_name,
                c.data_type,
                c.is_nullable,
                CASE WHEN pk.column_name IS NOT NULL THEN true ELSE false END AS is_primary_key,
                CASE WHEN c.column_default LIKE 'nextval(%' THEN true ELSE false END AS is_identity
            FROM information_schema.columns c
            LEFT JOIN (
                SELECT 
                    kcu.table_schema,
                    kcu.table_name,
                    kcu.column_name
                FROM information_schema.table_constraints tc
                JOIN information_schema.key_column_usage kcu 
                  ON tc.constraint_name = kcu.constraint_name 
                 AND tc.table_schema = kcu.table_schema
                WHERE tc.constraint_type = 'PRIMARY KEY'
            ) pk ON c.table_schema = pk.table_schema 
               AND c.table_name = pk.table_name 
               AND c.column_name = pk.column_name
            ORDER BY c.table_schema, c.table_name, c.ordinal_position";

        await using var command = new NpgsqlCommand(query, connection);
        await using var reader = await command.ExecuteReaderAsync();

        var rows = new List<(string Schema, string Table, ColumnSchema Col)>();

        while (await reader.ReadAsync())
        {
            var column = new ColumnSchema
            {
                Name = reader.GetString(2),
                DataType = MapDataType(reader.GetString(3)),
                IsNullable = reader.GetBoolean(4),
                IsPrimaryKey = reader.GetBoolean(5),
                IsIdentity = reader.GetBoolean(6)
            };

            rows.Add((reader.GetString(0), reader.GetString(1), column));
        }

        return rows.GroupBy(r => new { r.Schema, r.Table })
            .Select(g => new TableSchema
            {
                Name = $"{g.Key.Schema}.{g.Key.Table}",
                Columns = g.Select(x => x.Col).ToList()
            })
            .OrderBy(t => t.Name)
            .ToList();
    }

    public string GetCreateTableScript(TableSchema table)
    {
        var columns = string.Join(",\n    ", table.Columns.Select(c =>
        {
            var colDef = $"\"{EscapeIdentifier(c.Name)}\" {MapDataType(c.DataType)}";

            if (c.IsIdentity)
            {
                colDef += " GENERATED ALWAYS AS IDENTITY";
            }

            colDef += c.IsNullable ? " NULL" : " NOT NULL";
            return $"    {colDef}";
        }));

        var primaryKey = table.PrimaryKeys.Any()
            ? $",\n    CONSTRAINT \"PK_{table.Name.Replace(".", "_")}\" PRIMARY KEY ({string.Join(", ", table.PrimaryKeys.Select(k => $"\"{k}\""))})"
            : "";

        return $"CREATE TABLE \"{EscapeIdentifier(table.Name)}\" (\n{columns}{primaryKey}\n);";
    }

    public string GetDropTableScript(string tableName)
    {
        return $"DROP TABLE IF EXISTS \"{EscapeIdentifier(tableName)}\";";
    }

    public async Task CreateTableAsync(TableSchema table)
    {
        var script = GetCreateTableScript(table);
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DropTableAsync(string tableName)
    {
        var script = GetDropTableScript(tableName);
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
    }

    private static string EscapeIdentifier(string identifier)
    {
        return identifier.Replace("\"", "\"\"");
    }

    private static string MapDataType(string sqlServerType)
    {
        return sqlServerType.ToLower() switch
        {
            "int" => "INTEGER",
            "bigint" => "BIGINT",
            "smallint" => "SMALLINT",
            "bit" => "BOOLEAN",
            "uniqueidentifier" => "UUID",
            "datetime" or "datetime2" => "TIMESTAMP",
            "date" => "DATE",
            "time" => "TIME",
            "float" => "DOUBLE PRECISION",
            "real" => "REAL",
            "decimal" or "numeric" => "NUMERIC",
            "money" => "MONEY",
            "char" => "CHAR",
            "varchar" => "VARCHAR",
            "nchar" => "CHAR",
            "nvarchar" => "VARCHAR",
            "text" => "TEXT",
            "varbinary" => "BYTEA",
            "xml" => "XML",
            _ => sqlServerType
        };
    }
}