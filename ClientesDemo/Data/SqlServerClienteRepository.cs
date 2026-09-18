using ClientesDemo.Models;
using Microsoft.Data.SqlClient;

namespace ClientesDemo.Data;

/// <summary>
/// Repositório SQL Server — SQL parametrizado + script idempotente (AuthSoft-friendly).
/// </summary>
public sealed class SqlServerClienteRepository : IClienteRepository
{
    private readonly string _connectionString;

    public SqlServerClienteRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("Connection string SQL Server é obrigatória.", nameof(connectionString));

        _connectionString = connectionString;
    }

    public void Initialize()
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            """
            IF OBJECT_ID(N'dbo.clientes', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.clientes (
                    id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    nome NVARCHAR(200) NOT NULL,
                    documento NVARCHAR(50) NULL,
                    cidade NVARCHAR(120) NULL,
                    telefone NVARCHAR(40) NULL,
                    email NVARCHAR(200) NULL,
                    created_at DATETIME2 NOT NULL CONSTRAINT DF_clientes_created_at DEFAULT (SYSUTCDATETIME())
                );
            END
            """;
        cmd.ExecuteNonQuery();

        cmd.CommandText = "SELECT COUNT(*) FROM dbo.clientes;";
        var total = Convert.ToInt32(cmd.ExecuteScalar());
        if (total > 0)
            return;

        InserirSeed(conn, "Floricultura Encanto", "00.111.222/0001-33", "Rio Verde", "64 99999-1001", "contato@encanto.demo");
        InserirSeed(conn, "Casa Bella Decora", "11.222.333/0001-44", "Goiânia", "62 98888-2002", "compras@casabella.demo");
    }

    public IReadOnlyList<Cliente> Listar()
    {
        var lista = new List<Cliente>();
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            """
            SELECT id, nome, documento, cidade, telefone, email,
                   CONVERT(VARCHAR(33), created_at, 126) AS created_at
            FROM dbo.clientes
            ORDER BY id DESC;
            """;

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new Cliente
            {
                Id = reader.GetInt64(0),
                Nome = reader.GetString(1),
                Documento = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Cidade = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Telefone = reader.IsDBNull(4) ? "" : reader.GetString(4),
                Email = reader.IsDBNull(5) ? "" : reader.GetString(5),
                CreatedAt = reader.IsDBNull(6) ? "" : reader.GetString(6),
            });
        }

        return lista;
    }

    public void Salvar(Cliente cliente)
    {
        ArgumentNullException.ThrowIfNull(cliente);
        if (string.IsNullOrWhiteSpace(cliente.Nome))
            throw new ArgumentException("Informe o nome do cliente.", nameof(cliente));

        using var conn = Open();
        using var cmd = conn.CreateCommand();

        if (cliente.Id == 0)
        {
            cmd.CommandText =
                """
                INSERT INTO dbo.clientes (nome, documento, cidade, telefone, email)
                VALUES (@nome, @documento, @cidade, @telefone, @email);
                """;
        }
        else
        {
            cmd.CommandText =
                """
                UPDATE dbo.clientes
                SET nome = @nome,
                    documento = @documento,
                    cidade = @cidade,
                    telefone = @telefone,
                    email = @email
                WHERE id = @id;
                """;
            cmd.Parameters.AddWithValue("@id", cliente.Id);
        }

        cmd.Parameters.AddWithValue("@nome", cliente.Nome.Trim());
        cmd.Parameters.AddWithValue("@documento", (object?)cliente.Documento?.Trim() ?? "");
        cmd.Parameters.AddWithValue("@cidade", (object?)cliente.Cidade?.Trim() ?? "");
        cmd.Parameters.AddWithValue("@telefone", (object?)cliente.Telefone?.Trim() ?? "");
        cmd.Parameters.AddWithValue("@email", (object?)cliente.Email?.Trim() ?? "");
        cmd.ExecuteNonQuery();
    }

    public void Excluir(long id)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM dbo.clientes WHERE id = @id;";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }

    private SqlConnection Open()
    {
        var conn = new SqlConnection(_connectionString);
        conn.Open();
        return conn;
    }

    private static void InserirSeed(
        SqlConnection conn,
        string nome,
        string documento,
        string cidade,
        string telefone,
        string email)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            """
            INSERT INTO dbo.clientes (nome, documento, cidade, telefone, email)
            VALUES (@nome, @documento, @cidade, @telefone, @email);
            """;
        cmd.Parameters.AddWithValue("@nome", nome);
        cmd.Parameters.AddWithValue("@documento", documento);
        cmd.Parameters.AddWithValue("@cidade", cidade);
        cmd.Parameters.AddWithValue("@telefone", telefone);
        cmd.Parameters.AddWithValue("@email", email);
        cmd.ExecuteNonQuery();
    }
}
