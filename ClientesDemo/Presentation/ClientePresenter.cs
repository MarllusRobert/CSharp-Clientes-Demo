using ClientesDemo.Data;
using ClientesDemo.Models;

namespace ClientesDemo.Presentation;

/// <summary>
/// Presenter no padrão MVP: concentra regras de tela e orquestra o repositório.
/// </summary>
public sealed class ClientePresenter
{
    private readonly IClienteView _view;
    private readonly IClienteRepository _repository;

    public ClientePresenter(IClienteView view, IClienteRepository repository)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public void Inicializar()
    {
        try
        {
            _repository.Initialize();
            AtualizarLista();
            _view.ClearForm();
        }
        catch (Exception ex)
        {
            _view.ShowError("Falha ao iniciar o banco: " + ex.Message);
        }
    }

    public void Novo() => _view.ClearForm();

    public void AtualizarLista()
    {
        try
        {
            _view.BindClientes(_repository.Listar());
        }
        catch (Exception ex)
        {
            _view.ShowError("Falha ao listar clientes: " + ex.Message);
        }
    }

    public void Selecionar(Cliente? cliente)
    {
        if (cliente is null)
            return;

        _view.EditId = cliente.Id;
        _view.Nome = cliente.Nome;
        _view.Documento = cliente.Documento;
        _view.Cidade = cliente.Cidade;
        _view.Telefone = cliente.Telefone;
        _view.Email = cliente.Email;
    }

    public void Salvar()
    {
        try
        {
            var cliente = new Cliente
            {
                Id = _view.EditId,
                Nome = _view.Nome,
                Documento = _view.Documento,
                Cidade = _view.Cidade,
                Telefone = _view.Telefone,
                Email = _view.Email,
            };

            _repository.Salvar(cliente);
            AtualizarLista();
            _view.ClearForm();
            _view.ShowInfo("Cliente salvo com sucesso.");
        }
        catch (ArgumentException ex)
        {
            _view.ShowError(ex.Message);
        }
        catch (Exception ex)
        {
            _view.ShowError("Falha ao salvar: " + ex.Message);
        }
    }

    public void Excluir()
    {
        if (_view.EditId == 0)
        {
            _view.ShowError("Selecione um cliente na grade.");
            return;
        }

        if (!_view.Confirm("Excluir o cliente selecionado?"))
            return;

        try
        {
            _repository.Excluir(_view.EditId);
            AtualizarLista();
            _view.ClearForm();
        }
        catch (Exception ex)
        {
            _view.ShowError("Falha ao excluir: " + ex.Message);
        }
    }
}
