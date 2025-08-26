using Confluent.Kafka;
using Shared.Events;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace InvestSync.Api.src.Middleware
{
    public class StockPriceWebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<StockPriceWebSocketMiddleware> _logger;
        private static readonly ConcurrentBag<WebSocket> _connectedClients = new();
        private static bool _consumerStarted = false;
        private static readonly object _lockObject = new();

        public StockPriceWebSocketMiddleware(RequestDelegate next, ILogger<StockPriceWebSocketMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws/prices" && context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                _connectedClients.Add(webSocket);

                _logger.LogInformation("🔌 Cliente WebSocket conectado. Total: {Count}", _connectedClients.Count);

                // Iniciar consumer Kafka uma única vez
                StartKafkaConsumerOnce();

                // Manter conexão ativa
                await HandleWebSocketConnection(webSocket);
            }
            else
            {
                await _next(context);
            }
        }

        private void StartKafkaConsumerOnce()
        {
            lock (_lockObject)
            {
                if (!_consumerStarted)
                {
                    _consumerStarted = true;
                    _ = Task.Run(ConsumeKafkaPrices);
                    _logger.LogInformation("🚀 Consumer Kafka iniciado para preços");
                }
            }
        }

        private async Task ConsumeKafkaPrices()
        {
            var kafkaServers = Environment.GetEnvironmentVariable("Kafka__BootstrapServers") ?? "localhost:9092";
            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaServers,
                GroupId = "api-websocket-consumer",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe("precos.atualizados");

            _logger.LogInformation("📈 Consumindo preços do tópico precos.atualizados");

            while (true)
            {
                try
                {
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(1000));

                    if (result?.Message?.Value != null)
                    {
                        var preco = JsonSerializer.Deserialize<PrecoAtualizadoEvent>(result.Message.Value);

                        if (preco != null)
                        {
                            await BroadcastToClients(preco);
                            _logger.LogDebug("📊 Preço transmitido: {Ativo} - R$ {Preco}", preco.Ativo, preco.Preco);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao consumir preços do Kafka");
                    await Task.Delay(5000);
                }
            }
        }

        private async Task BroadcastToClients(PrecoAtualizadoEvent preco)
        {
            var message = JsonSerializer.Serialize(preco);
            var buffer = Encoding.UTF8.GetBytes(message);
            var clientsToRemove = new List<WebSocket>();

            foreach (var client in _connectedClients)
            {
                try
                {
                    if (client.State == WebSocketState.Open)
                    {
                        await client.SendAsync(
                            new ArraySegment<byte>(buffer),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None
                        );
                    }
                    else
                    {
                        clientsToRemove.Add(client);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "⚠️ Erro ao enviar para cliente WebSocket");
                    clientsToRemove.Add(client);
                }
            }

            // Remover clientes desconectados
            foreach (var client in clientsToRemove)
            {
                try
                {
                    client.Dispose();
                }
                catch { }
            }
        }

        private async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "⚠️ Conexão WebSocket encerrada");
            }
            finally
            {
                _logger.LogInformation("🔌 Cliente WebSocket desconectado");
            }
        }
    }
}
