using ClienteMS.Infraestrutura.Contexto;
using MassTransit;
using Polly;
using Polly.Retry;
using Rabbit.Dominio.Eventos;

namespace ClienteMS.Aplicacao.Servicos
{
    public class GerarPropostaConsumidor : IConsumer<PropostaFalhaEvento>
    {
        private readonly SqlContexto _sqlContexto;
        private readonly IPublishEndpoint _bus;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ILogger<GerarPropostaConsumidor> _logger;
        public GerarPropostaConsumidor(SqlContexto contexto, IPublishEndpoint bus, ILogger<GerarPropostaConsumidor> logger)
        {
            _sqlContexto = contexto;
            _logger = logger;
            _bus = bus;

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
        public async Task Consume(ConsumeContext<PropostaFalhaEvento> context)
        {
            await _retryPolicy.ExecuteAsync(() => _bus.Publish(new CriarPropostaEvento { IdCliente = context.Message.ClienteId }));
        }
    }
}
