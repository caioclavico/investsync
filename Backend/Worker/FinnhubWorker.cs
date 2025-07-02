using Confluent.Kafka;
using System.Text.Json;
using System.Reactive.Linq;
using Websocket.Client;
using Shared.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace InvestSync.FinnhubWorker;

public class FinnhubWorker : BackgroundService
{
    private readonly ILogger<FinnhubWorker> _logger;
    private WebsocketClient? _client;
    private readonly ConcurrentDictionary<string, bool> _ativosSubscritos = new();
    private readonly bool _useMockData;
    private readonly string[] _ativosMock = { "PETR4", "VALE3", "ITUB4" };

    public FinnhubWorker(ILogger<FinnhubWorker> logger)
    {
        _logger = logger;
        // Configurar via environment variable ou configuration
        _useMockData = Environment.GetEnvironmentVariable("USE_MOCK_DATA")?.ToLower() == "true";

        if (_useMockData)
        {
            _logger.LogInformation("🎭 Worker iniciado em modo MOCK - dados sintéticos serão gerados");
        }
        else
        {
            _logger.LogInformation("🌐 Worker iniciado em modo REAL - conectando ao Finnhub");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_useMockData)
        {
            await ExecuteMockDataAsync(stoppingToken);
        }
        else
        {
            await ExecuteRealDataAsync(stoppingToken);
        }
    }

    private async Task ExecuteMockDataAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🎭 Iniciando geração de dados mock...");

        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();
        var rand = new Random();

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var ativo in _ativosMock)
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

                Console.WriteLine($"🟢 Mock enviado: {ativo} - {preco.Preco}");

                await Task.Delay(2000, stoppingToken);
            }
        }
    }

    private async Task ExecuteRealDataAsync(CancellationToken stoppingToken)
    {
        var finnhubToken = "d1ar231r01qjhvtqrm1gd1ar231r01qjhvtqrm20"; // coloque seu token válido aqui

        var url = new Uri($"wss://ws.finnhub.io?token={finnhubToken}");
        _client = new WebsocketClient(url)
        {
            ReconnectTimeout = TimeSpan.FromSeconds(30)
        };

        var producerConfig = new ProducerConfig { BootstrapServers = "localhost:9092" };
        using var producer = new ProducerBuilder<string, string>(producerConfig).Build();

        var consumerConfig = new ConsumerConfig
        {
            GroupId = "finnhub-worker-group",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            SessionTimeoutMs = 10000,
            MaxPollIntervalMs = 300000
        };
        using var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

        // Configurar WebSocket
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
                        _logger.LogDebug($"Mensagem recebida do Finnhub: {msg.Text}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro processando mensagem do Finnhub");
                }
            });

        await _client.Start();

        // Aguarda conexão antes de processar eventos
        while (!_client.IsRunning)
        {
            await Task.Delay(100, stoppingToken);
        }

        // Configura o consumer para o tópico de subscrição de ativos
        consumer.Subscribe("ativos.subscricao");
        _logger.LogInformation("Consumer configurado para escutar o tópico 'ativos.subscricao'");

        // Task para processar mensagens do consumer
        var consumerTask = Task.Run(async () =>
        {
            _logger.LogInformation("🔄 Consumer task iniciada, aguardando mensagens...");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(1000));
                    if (result != null)
                    {
                        _logger.LogInformation($"📦 Mensagem recebida do Kafka: {result.Message.Value}");
                        await ProcessarEventoSubscricao(result.Message.Value);

                        consumer.Commit(result);
                        _logger.LogDebug("✅ Mensagem confirmada no Kafka");
                    }
                    else
                    {
                        _logger.LogDebug("⏳ Aguardando mensagens no tópico 'ativos.subscricao'...");
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "❌ Erro específico do Kafka consumer");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro geral processando evento de subscrição");
                }
            }

            _logger.LogInformation("🛑 Consumer task finalizada");
        }, stoppingToken);

        // Aguarda cancelamento
        await Task.Delay(Timeout.Infinite, stoppingToken);
        await consumerTask;
    }

    private async Task SubscribeToAtivo(string ativo)
    {
        if (_client?.IsRunning == true && !_ativosSubscritos.ContainsKey(ativo))
        {
            var msg = JsonSerializer.Serialize(new { type = "subscribe", symbol = ativo });
            await _client.SendInstant(msg);
            _ativosSubscritos.TryAdd(ativo, true);
            _logger.LogInformation($"✅ Subscrito no ativo: {ativo}");
        }
    }

    private async Task UnsubscribeFromAtivo(string ativo)
    {
        if (_client?.IsRunning == true && _ativosSubscritos.ContainsKey(ativo))
        {
            var msg = JsonSerializer.Serialize(new { type = "unsubscribe", symbol = ativo });
            await _client.SendInstant(msg);
            _ativosSubscritos.TryRemove(ativo, out _);
            _logger.LogInformation($"❌ Dessubscrito do ativo: {ativo}");
        }
    }

    private async Task ProcessarEventoSubscricao(string eventoJson)
    {
        try
        {
            _logger.LogInformation($"🔍 Processando evento JSON: {eventoJson}");

            // Verificar se é uma mensagem válida antes de deserializar
            if (string.IsNullOrWhiteSpace(eventoJson))
            {
                _logger.LogWarning("⚠️ Evento JSON está vazio ou nulo");
                return;
            }

            AtivoSubscricaoEvent? evento;
            try
            {
                evento = JsonSerializer.Deserialize<AtivoSubscricaoEvent>(eventoJson);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "⚠️ Mensagem não é um AtivoSubscricaoEvent válido, ignorando: {EventoJson}", eventoJson);
                return;
            }

            if (evento == null)
            {
                _logger.LogWarning("⚠️ Evento de subscrição nulo após deserialização");
                return;
            }

            _logger.LogInformation($"📨 Evento deserializado - Ação: '{evento.Acao}' | Ativo: '{evento.Ativo}' | Timestamp: {evento.Timestamp}");

            // Ignorar mensagens de teste
            if (evento.Acao.ToLower() == "test")
            {
                _logger.LogInformation($"🧪 Mensagem de teste recebida e ignorada para ativo: {evento.Ativo}");
                return;
            }

            switch (evento.Acao.ToLower())
            {
                case "subscribe":
                    _logger.LogInformation($"➡️ Executando subscrição para {evento.Ativo}");
                    await SubscribeToAtivo(evento.Ativo);
                    break;
                case "unsubscribe":
                    _logger.LogInformation($"⬅️ Executando dessubscrição para {evento.Ativo}");
                    await UnsubscribeFromAtivo(evento.Ativo);
                    break;
                default:
                    _logger.LogWarning($"❓ Ação desconhecida recebida: '{evento.Acao}'");
                    break;
            }

            _logger.LogInformation($"✅ Processamento do evento concluído para {evento.Ativo}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Erro geral ao processar evento de subscrição: {EventoJson}", eventoJson);
        }
    }

    public override void Dispose()
    {
        _client?.Dispose();
        base.Dispose();
    }
}