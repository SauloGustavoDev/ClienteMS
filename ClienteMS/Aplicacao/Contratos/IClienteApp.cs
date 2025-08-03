using ClienteMS.Dominio.Models;
using ClienteMS.Dominio.Models.DTOs;
using ClienteMS.Dominio.Models.Request;
using Shared.Modelos;

namespace ClienteMS.Aplicacao.Contratos
{
    public interface IClienteApp
    {
        Task<ReturnoDto<Cliente>> CriarClienteAsync(ClienteRequest cliente);

    }
}
