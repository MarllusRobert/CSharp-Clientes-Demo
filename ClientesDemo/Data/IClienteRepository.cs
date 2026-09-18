using ClientesDemo.Models;

namespace ClientesDemo.Data;

public interface IClienteRepository
{
    void Initialize();
    IReadOnlyList<Cliente> Listar();
    void Salvar(Cliente cliente);
    void Excluir(long id);
}
