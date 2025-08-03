using CartaoMS.Dominio.Eventos;
using ClienteMS.Aplicacao.Contratos;
using ClienteMS.Dominio.Models;
using ClienteMS.Dominio.Models.DTOs;
using ClienteMS.Dominio.Models.Request;
using ClienteMS.Infraestrutura.Contexto;
using MassTransit;
using Polly;
using Polly.Retry;
using Shared.Modelos;

namespace ClienteMS.Aplicacao.Servicos
{
    public class ClienteApp : IClienteApp
    {
        private readonly SqlContexto _sqlContexto;
        private readonly IPublishEndpoint _bus;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ILogger<ClienteApp> _logger;


        public ClienteApp(SqlContexto contexto, IPublishEndpoint bus, ILogger<ClienteApp> logger)
        {
            _sqlContexto = contexto;
            _bus = bus;
            _logger = logger;


            _retryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(2),
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception, $"Erro ao publicar evento (tentativa {retryCount}). Tentando novamente em {timeSpan.TotalSeconds} segundos.");
                    });
        }

        public async Task<ReturnoDto<Cliente>> CriarClienteAsync(ClienteRequest cliente)
        {
            var clienteFormatado = MapearCliente(cliente);
            try
            {
                await _sqlContexto.AddAsync(clienteFormatado);
                await _sqlContexto.SaveChangesAsync();

                // Usando Polly para garantir resiliência na publicação
                await _retryPolicy.ExecuteAsync(() => _bus.Publish(new ClienteCriadoEvento {Id = clienteFormatado.Id, SimularErro = cliente.SimularErro }));
                _logger.LogInformation($"Cliente cadastrado e evento publicado com sucesso. ClienteId: {clienteFormatado.Id}");

                return new ReturnoDto<Cliente>()
                {
                    Dados = clienteFormatado,
                    Sucesso = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar cliente ou publicar evento. ClienteId: {ClienteId}", clienteFormatado.Id);

                return new ReturnoDto<Cliente>()
                {
                    Dados = clienteFormatado,
                    Sucesso = false,
                    Excecao = ex,
                    Mensagem = "Erro ao cadastrar cliente ou publicar evento: " + ex.Message
                };
            }
        }

        private Cliente MapearCliente(ClienteRequest cliente)
        {
            return new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = cliente.Nome,
                Renda = cliente.Renda,
            };
        }
    }
}
