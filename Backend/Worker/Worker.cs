using Confluent.Kafka;
using System.Text.Json;
using Shared.Events;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger) => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();

        var ativos = new[] { "PETR4", "VALE3", "ITUB4" };
        var rand = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var ativo in ativos)
            {
                var preco = new PrecoAtualizadoEvent
                {
                    Ativo = ativo,
                    Preco = (decimal)Math.Round(10 + rand.NextDouble() * 90, 2),
                    Timestamp = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(preco);

                await producer.ProduceAsync("precos.atualizados", new Message<string, string>
                {
                    Key = ativo,
                    Value = json
                });

                // _logger.LogInformation($"🟢 Enviado: {ativo} - {preco.Preco}");
                Console.WriteLine($"🟢 Enviado: {ativo} - {preco.Preco}");

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}
