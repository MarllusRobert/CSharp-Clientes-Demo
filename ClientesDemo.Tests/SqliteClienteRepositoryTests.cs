using ClientesDemo.Data;
using ClientesDemo.Models;

namespace ClientesDemo.Tests;

public class SqliteClienteRepositoryTests : IDisposable
{
    private readonly string _dbPath;
    private readonly SqliteClienteRepository _repo;

    public SqliteClienteRepositoryTests()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"clientes-test-{Guid.NewGuid():N}.db");
        _repo = new SqliteClienteRepository(_dbPath);
        _repo.Initialize();
    }

    public void Dispose()
    {
        try
        {
            if (File.Exists(_dbPath))
                File.Delete(_dbPath);
        }
        catch
        {
            // ignore temp cleanup
        }
    }

    [Fact]
    public void Initialize_SeedsTwoClientes()
    {
        var lista = _repo.Listar();
        Assert.Equal(2, lista.Count);
    }

    [Fact]
    public void Salvar_Insert_AddsCliente()
    {
        _repo.Salvar(new Cliente
        {
            Nome = "Cliente Teste",
            Documento = "123",
            Cidade = "Rio Verde",
            Telefone = "64 90000-0000",
            Email = "teste@demo.local",
        });

        var lista = _repo.Listar();
        Assert.Equal(3, lista.Count);
        Assert.Contains(lista, c => c.Nome == "Cliente Teste");
    }

    [Fact]
    public void Salvar_WithoutNome_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            _repo.Salvar(new Cliente { Nome = "   " }));
    }

    [Fact]
    public void Excluir_RemovesCliente()
    {
        var antes = _repo.Listar().First();
        _repo.Excluir(antes.Id);
        var depois = _repo.Listar();
        Assert.DoesNotContain(depois, c => c.Id == antes.Id);
    }

    [Fact]
    public void Salvar_Update_ChangesNome()
    {
        var item = _repo.Listar().First();
        item.Nome = "Nome Atualizado";
        _repo.Salvar(item);

        var updated = _repo.Listar().First(c => c.Id == item.Id);
        Assert.Equal("Nome Atualizado", updated.Nome);
    }
}
