using CartaoMS.Dominio.Eventos;
using ClienteMS.Dominio.Eventos;
using ClienteMS.Infraestrutura.Contexto;
using ClienteMS.Modelos;
using MassTransit;
using Polly;
using Polly.Retry;

namespace ClienteMS.Aplicacao.Servicos
{
    public class GerarCartaoConsumidor : IConsumer<CartaoFalhaEvento>
    {
        private readonly SqlContexto _sqlContexto;
        private readonly IPublishEndpoint _bus;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly ILogger<GerarCartaoConsumidor> _logger;

        public GerarCartaoConsumidor(SqlContexto contexto, IPublishEndpoint bus, ILogger<GerarCartaoConsumidor> logger)
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
        public async Task Consume(ConsumeContext<CartaoFalhaEvento> context)
        {
            await _retryPolicy.ExecuteAsync(() => _bus.Publish(new CriarCartaoEvento { idCliente = context.Message.ClienteId}));
        }
    }
}
