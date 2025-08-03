using CartaoMS.Dominio.Eventos;
using MassTransit;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;

namespace ClienteMS.Aplicacao.Servicos
{
    public static class RabbitMqApp
    {
        public static void AddRabbitMqApp(this IServiceCollection services, IConfiguration _config)
        {
            var retryPolicy = Policy
                            .Handle<Exception>()
                            .WaitAndRetry(3, retry => TimeSpan.FromSeconds(2));

            retryPolicy.Execute(() => TestaRabbitConnection(_config));


            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.AddConsumer<GerarPropostaConsumidor>();
                busConfigurator.AddConsumer<GerarCartaoConsumidor>();

                busConfigurator.UsingRabbitMq((ctx, cfg) => {
                    cfg.Host(new Uri(_config["RabbitConnection:host"]), host =>
                    {
                        host.Username(_config["RabbitConnection:username"]);
                        host.Password(_config["RabbitConnection:password"]);
                    });

                    cfg.ReceiveEndpoint("regerar-proposta-cliente", e =>
                    {
                        e.ConfigureConsumer<GerarPropostaConsumidor>(ctx);
                    });
                    cfg.ReceiveEndpoint("regerar-cartao-cliente", e =>
                    {
                        e.ConfigureConsumer<GerarCartaoConsumidor>(ctx);
                    });
                });
            });
        }

        private static void TestaRabbitConnection(IConfiguration config)
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(config["RabbitConnection:host"]),
                UserName = config["RabbitConnection:username"],
                Password = config["RabbitConnection:password"]
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
        }
    }
}
