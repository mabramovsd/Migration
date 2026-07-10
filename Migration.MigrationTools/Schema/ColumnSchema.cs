namespace Migration.MigrationTools.Schema;

public class ColumnSchema
{
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsIdentity { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
}
