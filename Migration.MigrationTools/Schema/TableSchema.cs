namespace Migration.MigrationTools.Schema;

public class TableSchema
{
    public string Name { get; set; } = string.Empty;
    public List<ColumnSchema> Columns { get; set; } = new();
    public List<string> PrimaryKeys { get; set; } = new();
}
