using ClienteMS.Aplicacao.Contratos;
using ClienteMS.Dominio.Models.DTOs;
using ClienteMS.Dominio.Models.Request;
using ClienteMS.Infraestrutura.Contexto;
using ClienteMS.Modelos;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Retry;
using Rabbit.Dominio.Eventos;

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

        public async Task GerarCartaoAsync(Guid id)
        {
            await _retryPolicy.ExecuteAsync(() => _bus.Publish(new CriarCartaoEvento { idCliente = id }));
        }

        public async Task<List<Cliente>> GetClientesAsync()
        {
            var clientes = await _sqlContexto.Set<Cliente>()
                .AsNoTracking()
                .Include(x => x.Cartao)
                .Include(x => x.Proposta)
                .ToListAsync();
            return clientes;
        }

        public async Task<Cliente?> GetClienteAsync(Guid id)
        {
            var cliente = await _sqlContexto.Set<Cliente>()
                .AsNoTracking()
                .Include(x => x.Cartao)
                .Include(x => x.Proposta)
                .FirstOrDefaultAsync(x => x.Id == id);
            return cliente;
        }

        public async Task<ReturnoDto<Cliente>> CriarClienteAsync(ClienteRequest cliente)
        {
            var clienteFormatado = MapearCliente(cliente);
            try
            {
                await _retryPolicy.ExecuteAsync(async () =>
                {
                    await _sqlContexto.AddAsync(clienteFormatado);
                    await _sqlContexto.SaveChangesAsync();
                });
                // Usando Polly para garantir resiliência na publicação
                await _retryPolicy.ExecuteAsync(() => _bus.Publish(new ClienteCriadoEvento { Id = clienteFormatado.Id, SimularErro = cliente.SimularErro }));
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
                await _sqlContexto.AddAsync(SalvaErro(clienteFormatado.Id, clienteFormatado.GetType().Name, ex.Message, ex.StackTrace));
                await _retryPolicy.ExecuteAsync(() => _sqlContexto.SaveChangesAsync());

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

        private Erro SalvaErro(Guid id, string tipo, string mensagem, string trace)
        {
            return new Erro
            {
                Id = Guid.NewGuid(),
                ClienteId = id,
                Tipo = tipo,
                DtErro = DateTime.Now,
                Mensagem = mensagem,
                StackTrace = trace
            };
        }
    }
}
