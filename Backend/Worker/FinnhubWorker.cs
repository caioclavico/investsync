using Confluent.Kafka;
using System.Text.Json;
using System.Reactive.Linq;
using Websocket.Client;
using Shared.Events;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace InvestSync.Worker;

public class FinnhubWorker : BackgroundService
{
    private readonly ILogger<FinnhubWorker> _logger;
    private WebsocketClient? _client;
    private readonly ConcurrentDictionary<string, bool> _ativosSubscritos = new();

    public FinnhubWorker(ILogger<FinnhubWorker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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
            AutoOffsetReset = AutoOffsetReset.Earliest, // Mudei para ler desde o início
            EnableAutoCommit = false, // Controle manual de commit
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
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(1000)); // Aumentei timeout para 1 segundo
                    if (result != null)
                    {
                        _logger.LogInformation($"📦 Mensagem recebida do Kafka: {result.Message.Value}");
                        await ProcessarEventoSubscricao(result.Message.Value);

                        // Confirmar o processamento da mensagem
                        consumer.Commit(result);
                        _logger.LogDebug("✅ Mensagem confirmada no Kafka");
                    }
                    else
                    {
                        // Log periódico para mostrar que está vivo
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

            var evento = JsonSerializer.Deserialize<AtivoSubscricaoEvent>(eventoJson);
            if (evento == null)
            {
                _logger.LogWarning("⚠️ Evento de subscrição nulo recebido");
                return;
            }

            _logger.LogInformation($"📨 Evento deserializado - Ação: '{evento.Acao}' | Ativo: '{evento.Ativo}' | Timestamp: {evento.Timestamp}");

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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "❌ Erro de deserialização JSON: {EventoJson}", eventoJson);
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