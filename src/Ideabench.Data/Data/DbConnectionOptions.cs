namespace Ideabench.Data.Data;

public sealed class DbConnectionOptions
{
    public const string SectionName = "ConnectionStrings";

    public string DefaultConnection { get; set; } = "Data Source=ideabench.db";
}
