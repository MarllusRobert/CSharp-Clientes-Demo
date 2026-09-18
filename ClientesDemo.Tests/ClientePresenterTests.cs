using ClientesDemo.Models;
using ClientesDemo.Presentation;

namespace ClientesDemo.Tests;

public class ClientePresenterTests
{
    [Fact]
    public void Salvar_WithEmptyNome_ShowsError_AndDoesNotPersist()
    {
        var view = new FakeView();
        var repo = new FakeRepo();
        var presenter = new ClientePresenter(view, repo);

        view.Nome = "";
        presenter.Salvar();

        Assert.Contains("nome", view.LastError, StringComparison.OrdinalIgnoreCase);
        Assert.Empty(repo.Items);
    }

    [Fact]
    public void Salvar_WithNome_Persists_AndClearsForm()
    {
        var view = new FakeView { Nome = "Nova Empresa", Cidade = "Rio Verde" };
        var repo = new FakeRepo();
        var presenter = new ClientePresenter(view, repo);

        presenter.Salvar();

        Assert.Single(repo.Items);
        Assert.Equal("Nova Empresa", repo.Items[0].Nome);
        Assert.Equal(0, view.EditId);
        Assert.Equal("", view.Nome);
        Assert.Contains("sucesso", view.LastInfo, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Excluir_WithoutSelection_ShowsError()
    {
        var view = new FakeView { EditId = 0 };
        var repo = new FakeRepo();
        var presenter = new ClientePresenter(view, repo);

        presenter.Excluir();

        Assert.Contains("Selecione", view.LastError, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FakeView : IClienteView
    {
        public string Nome { get; set; } = "";
        public string Documento { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string Telefone { get; set; } = "";
        public string Email { get; set; } = "";
        public long EditId { get; set; }
        public string LastError { get; private set; } = "";
        public string LastInfo { get; private set; } = "";

        public void BindClientes(IReadOnlyList<Cliente> clientes) { }
        public void ClearForm()
        {
            EditId = 0;
            Nome = Documento = Cidade = Telefone = Email = "";
        }
        public void ShowInfo(string message) => LastInfo = message;
        public void ShowError(string message) => LastError = message;
        public bool Confirm(string message) => true;
    }

    private sealed class FakeRepo : ClientesDemo.Data.IClienteRepository
    {
        public List<Cliente> Items { get; } = new();
        private long _seq = 1;

        public void Initialize() { }
        public IReadOnlyList<Cliente> Listar() => Items.OrderByDescending(i => i.Id).ToList();

        public void Salvar(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.Nome))
                throw new ArgumentException("Informe o nome do cliente.");

            if (cliente.Id == 0)
            {
                cliente.Id = _seq++;
                Items.Add(new Cliente
                {
                    Id = cliente.Id,
                    Nome = cliente.Nome,
                    Documento = cliente.Documento,
                    Cidade = cliente.Cidade,
                    Telefone = cliente.Telefone,
                    Email = cliente.Email,
                });
            }
            else
            {
                var idx = Items.FindIndex(i => i.Id == cliente.Id);
                if (idx >= 0)
                    Items[idx] = cliente;
            }
        }

        public void Excluir(long id) => Items.RemoveAll(i => i.Id == id);
    }
}
