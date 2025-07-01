using Confluent.Kafka;
using System.Text.Json;
using System.Reactive.Linq;
using Websocket.Client;
using Shared.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class FinnhubWorker : BackgroundService
{
    private readonly ILogger<FinnhubWorker> _logger;
    private WebsocketClient? _client;

    public FinnhubWorker(ILogger<FinnhubWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var finnhubToken = "d1ar231r01qjhvtqrm1gd1ar231r01qjhvtqrm20"; // coloque seu token válido aqui
        var ativos = new[] { "PETR4", "VALE3", "ITUB4", "AAPL" };

        var url = new Uri($"wss://ws.finnhub.io?token={finnhubToken}");
        _client = new WebsocketClient(url)
        {
            ReconnectTimeout = TimeSpan.FromSeconds(30)
        };

        var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
        using var producer = new ProducerBuilder<string, string>(config).Build();

        _client.ReconnectionHappened.Subscribe(info =>
            _logger.LogInformation($"WebSocket reconectado: {info.Type}"));
        _client.DisconnectionHappened.Subscribe(info =>
            _logger.LogWarning($"WebSocket desconectado: {info.Type}"));

        _client.MessageReceived
            .Where(m => !string.IsNullOrWhiteSpace(m.Text))
            .Subscribe(async msg =>
            {
                try
                {
                    if (msg.Text == null)
                        return;

                    using var parsed = JsonDocument.Parse(msg.Text);
                    if (parsed.RootElement.TryGetProperty("type", out var type) && type.GetString() == "trade")
                    {
                        foreach (var trade in parsed.RootElement.GetProperty("data").EnumerateArray())
                        {
                            var preco = new PrecoAtualizadoEvent
                            {
                                Ativo = trade.GetProperty("s").GetString() ?? "",
                                Preco = (decimal)trade.GetProperty("p").GetDouble(),
                                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(trade.GetProperty("t").GetInt64()).UtcDateTime
                            };

                            var json = JsonSerializer.Serialize(preco);

                            await producer.ProduceAsync("precos.atualizados", new Message<string, string>
                            {
                                Key = preco.Ativo,
                                Value = json
                            });

                            Console.WriteLine($"🌐 Enviado (REAL): {preco.Ativo} - {preco.Preco}");
                        }
                    }
                    else
                    {
                        // Log para mensagens não "trade" (útil para debug)
                        _logger.LogDebug($"Mensagem recebida do Finnhub: {msg.Text}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro processando mensagem do Finnhub");
                }
            });

        await _client.Start();

        // Aguarda conexão antes de enviar os subscribes
        while (!_client.IsRunning)
        {
            await Task.Delay(100, stoppingToken);
        }

        foreach (var ativo in ativos)
        {
            var msg = JsonSerializer.Serialize(new { type = "subscribe", symbol = ativo });
            await _client.SendInstant(msg);
            _logger.LogInformation($"Enviado subscribe para {ativo}");
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }

    public override void Dispose()
    {
        _client?.Dispose();
        base.Dispose();
    }
}