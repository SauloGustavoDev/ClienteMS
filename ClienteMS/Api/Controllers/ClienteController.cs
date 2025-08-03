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


        [HttpGet("GetClientes")]
        public async Task<ActionResult> GetClientes()
        {
            var result = await _clienteApp.GetClientesAsync();
            return Ok(result);
        }

        [HttpGet("GetCliente")]
        public async Task<ActionResult> GetCliente([FromBody]Guid id)
        {
            var result = await _clienteApp.GetClienteAsync(id);
            return Ok(result); 
        }

        [HttpPost("GerarCartaoCliente")]
        public async Task<ActionResult> GerarCartaoCliente([FromBody]Guid idCliente)
        {
            await _clienteApp.GerarCartaoAsync(idCliente);
            return Ok();
        }
    }
}
