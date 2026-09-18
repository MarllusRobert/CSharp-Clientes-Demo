using ClientesDemo.Models;
using Microsoft.Data.Sqlite;

namespace ClientesDemo.Data;

public sealed class SqliteClienteRepository : IClienteRepository
{
    private readonly string _connectionString;

    public SqliteClienteRepository(string databasePath)
    {
        var dir = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(dir))
            Directory.CreateDirectory(dir);

        _connectionString = $"Data Source={databasePath}";
    }

    public void Initialize()
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            """
            CREATE TABLE IF NOT EXISTS clientes (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                nome TEXT NOT NULL,
                documento TEXT,
                cidade TEXT,
                telefone TEXT,
                email TEXT,
                created_at TEXT DEFAULT CURRENT_TIMESTAMP
            );
            """;
        cmd.ExecuteNonQuery();

        cmd.CommandText = "SELECT COUNT(*) FROM clientes;";
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
            SELECT id, nome, documento, cidade, telefone, email, created_at
            FROM clientes
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
                INSERT INTO clientes (nome, documento, cidade, telefone, email)
                VALUES ($nome, $documento, $cidade, $telefone, $email);
                """;
        }
        else
        {
            cmd.CommandText =
                """
                UPDATE clientes
                SET nome = $nome,
                    documento = $documento,
                    cidade = $cidade,
                    telefone = $telefone,
                    email = $email
                WHERE id = $id;
                """;
            cmd.Parameters.AddWithValue("$id", cliente.Id);
        }

        cmd.Parameters.AddWithValue("$nome", cliente.Nome.Trim());
        cmd.Parameters.AddWithValue("$documento", cliente.Documento?.Trim() ?? "");
        cmd.Parameters.AddWithValue("$cidade", cliente.Cidade?.Trim() ?? "");
        cmd.Parameters.AddWithValue("$telefone", cliente.Telefone?.Trim() ?? "");
        cmd.Parameters.AddWithValue("$email", cliente.Email?.Trim() ?? "");
        cmd.ExecuteNonQuery();
    }

    public void Excluir(long id)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM clientes WHERE id = $id;";
        cmd.Parameters.AddWithValue("$id", id);
        cmd.ExecuteNonQuery();
    }

    private SqliteConnection Open()
    {
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        return conn;
    }

    private static void InserirSeed(
        SqliteConnection conn,
        string nome,
        string documento,
        string cidade,
        string telefone,
        string email)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            """
            INSERT INTO clientes (nome, documento, cidade, telefone, email)
            VALUES ($nome, $documento, $cidade, $telefone, $email);
            """;
        cmd.Parameters.AddWithValue("$nome", nome);
        cmd.Parameters.AddWithValue("$documento", documento);
        cmd.Parameters.AddWithValue("$cidade", cidade);
        cmd.Parameters.AddWithValue("$telefone", telefone);
        cmd.Parameters.AddWithValue("$email", email);
        cmd.ExecuteNonQuery();
    }
}
