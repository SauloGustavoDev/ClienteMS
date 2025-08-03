using ClienteMS.Dominio.Models.DTOs;
using ClienteMS.Dominio.Models.Request;
using ClienteMS.Modelos;

namespace ClienteMS.Aplicacao.Contratos
{
    public interface IClienteApp
    {
        Task<ReturnoDto<Cliente>> CriarClienteAsync(ClienteRequest cliente);
        Task<List<Cliente?>> GetClientesAsync();
        Task<Cliente?> GetClienteAsync(Guid id);
        Task GerarCartaoAsync(Guid id);
    }
}
