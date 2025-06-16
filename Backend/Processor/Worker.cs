using Confluent.Kafka;
using Shared.Events;
using System.Text.Json;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private static readonly Dictionary<string, PrecoAtualizadoEvent> _precos = new();

    public Worker(ILogger<Worker> logger) => _logger = logger;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() =>
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "investsync-processor",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("precos.atualizados");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(stoppingToken);
                    var evento = JsonSerializer.Deserialize<PrecoAtualizadoEvent>(cr.Message.Value);

                    if (evento != null)
                    {
                        _precos[evento.Ativo] = evento;
                        // _logger.LogInformation($"🟡 Consumido: {evento.Ativo} - {evento.Preco}");
                        Console.WriteLine($"🟡 Consumido: {evento.Ativo} - {evento.Preco}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao consumir evento");
                }
            }
        });
    }
}
