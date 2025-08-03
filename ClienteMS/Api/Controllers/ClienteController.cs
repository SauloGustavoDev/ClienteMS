using ClienteMS.Aplicacao.Contratos;
using ClienteMS.Dominio.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace ClienteMS.Api.Controllers
{
    public class ClienteController : ControllerBase
    {
        private readonly IClienteApp _clienteApp;
        public ClienteController(IClienteApp cliente)
        {
            _clienteApp = cliente;
        }

        [HttpPost("CriarCliente")]
        public async Task<ActionResult> CriarCliente([FromBody] ClienteRequest cliente)
        {
            var resultado = await _clienteApp.CriarClienteAsync(cliente);

            if (!resultado.Sucesso)
                return BadRequest(resultado.Mensagem);

            return Ok(); ;
        }
    }
}
