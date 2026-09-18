using ClientesDemo.Models;

namespace ClientesDemo.Presentation;

public interface IClienteView
{
    string Nome { get; set; }
    string Documento { get; set; }
    string Cidade { get; set; }
    string Telefone { get; set; }
    string Email { get; set; }
    long EditId { get; set; }

    void BindClientes(IReadOnlyList<Cliente> clientes);
    void ClearForm();
    void ShowInfo(string message);
    void ShowError(string message);
    bool Confirm(string message);
}
