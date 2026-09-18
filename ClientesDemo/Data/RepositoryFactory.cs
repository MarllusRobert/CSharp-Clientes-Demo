namespace ClientesDemo.Data;

public static class RepositoryFactory
{
    public const string ProviderSqlite = "sqlite";
    public const string ProviderSqlServer = "sqlserver";

    /// <summary>
    /// Provider via variável CLIENTES_DB_PROVIDER (sqlite|sqlserver) ou appsettings.json.
    /// SQL Server: CLIENTES_SQLSERVER_CONNECTION ou ConnectionStrings:SqlServer.
    /// </summary>
    public static (IClienteRepository Repository, string ProviderLabel) Create()
    {
        var provider = (Environment.GetEnvironmentVariable("CLIENTES_DB_PROVIDER")
                        ?? ReadAppSetting("Database:Provider")
                        ?? ProviderSqlite)
            .Trim()
            .ToLowerInvariant();

        if (provider is ProviderSqlServer or "mssql" or "sql")
        {
            var cs = Environment.GetEnvironmentVariable("CLIENTES_SQLSERVER_CONNECTION")
                     ?? ReadAppSetting("ConnectionStrings:SqlServer")
                     ?? "Server=(localdb)\\MSSQLLocalDB;Database=ClientesDemo;Trusted_Connection=True;TrustServerCertificate=True;";

            return (new SqlServerClienteRepository(cs), "SQL Server");
        }

        var dbPath = Path.Combine(AppContext.BaseDirectory, "data", "clientes.db");
        return (new SqliteClienteRepository(dbPath), "SQLite");
    }

    private static string? ReadAppSetting(string key)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(path))
            return null;

        try
        {
            var json = File.ReadAllText(path);
            // parser mínimo sem dependência extra
            if (key == "Database:Provider")
                return ExtractJsonString(json, "Provider");
            if (key == "ConnectionStrings:SqlServer")
                return ExtractJsonString(json, "SqlServer");
        }
        catch
        {
            // ignora — cai no default
        }

        return null;
    }

    private static string? ExtractJsonString(string json, string property)
    {
        var token = $"\"{property}\"";
        var idx = json.IndexOf(token, StringComparison.OrdinalIgnoreCase);
        if (idx < 0)
            return null;
        var colon = json.IndexOf(':', idx);
        if (colon < 0)
            return null;
        var firstQuote = json.IndexOf('"', colon + 1);
        if (firstQuote < 0)
            return null;
        var secondQuote = json.IndexOf('"', firstQuote + 1);
        if (secondQuote < 0)
            return null;
        return json.Substring(firstQuote + 1, secondQuote - firstQuote - 1);
    }
}
