using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using System.Text.Json;
using Shared.Events;
using InvestSync.Api.src.DTOs;

namespace InvestSync.Api.src.Controllers
{
    [ApiController]
    [Route("subscriptions")]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ILogger<SubscriptionsController> _logger;

        public SubscriptionsController(ILogger<SubscriptionsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("subscribe/{ativo}")]
        public async Task<IActionResult> Subscribe(string ativo)
        {
            _logger.LogInformation("🎯 Endpoint Subscribe chamado - Ativo: {Ativo}", ativo);

            try
            {
                if (string.IsNullOrWhiteSpace(ativo))
                {
                    _logger.LogWarning("⚠️ Nome do ativo é nulo ou vazio");
                    return BadRequest(ApiResponse<object>.CreateError("Nome do ativo é obrigatório"));
                }

                _logger.LogInformation("📞 Chamando PublishSubscriptionEvent...");
                await PublishSubscriptionEvent(ativo, "subscribe");

                _logger.LogInformation("✅ Subscrição processada com sucesso para {Ativo}", ativo);
                return Ok(ApiResponse<object>.CreateSuccess(
                    new { ativo, acao = "subscribe" },
                    $"Subscrição enviada para o ativo {ativo}"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao processar subscrição para ativo {Ativo}", ativo);
                return StatusCode(500, ApiResponse<object>.CreateError($"Erro interno do servidor: {ex.Message}"));
            }
        }

        [HttpPost("unsubscribe/{ativo}")]
        public async Task<IActionResult> Unsubscribe(string ativo)
        {
            _logger.LogInformation("🎯 Endpoint Unsubscribe chamado - Ativo: {Ativo}", ativo);

            try
            {
                if (string.IsNullOrWhiteSpace(ativo))
                {
                    _logger.LogWarning("⚠️ Nome do ativo é nulo ou vazio");
                    return BadRequest(ApiResponse<object>.CreateError("Nome do ativo é obrigatório"));
                }

                _logger.LogInformation("📞 Chamando PublishSubscriptionEvent...");
                await PublishSubscriptionEvent(ativo, "unsubscribe");

                _logger.LogInformation("✅ Dessubscrição processada com sucesso para {Ativo}", ativo);
                return Ok(ApiResponse<object>.CreateSuccess(
                    new { ativo, acao = "unsubscribe" },
                    $"Dessubscrição enviada para o ativo {ativo}"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao processar dessubscrição para ativo {Ativo}", ativo);
                return StatusCode(500, ApiResponse<object>.CreateError($"Erro interno do servidor: {ex.Message}"));
            }
        }

        [HttpGet("test-kafka")]
        [AllowAnonymous]
        public async Task<IActionResult> TestKafka()
        {
            _logger.LogInformation("🧪 Testando conexão com Kafka...");

            try
            {
                var config = new ProducerConfig
                {
                    BootstrapServers = "localhost:9092",
                    MessageTimeoutMs = 5000,
                    RequestTimeoutMs = 5000
                };

                using var producer = new ProducerBuilder<string, string>(config).Build();

                var testMessage = new Message<string, string>
                {
                    Key = "test",
                    Value = "test-connection"
                };

                var result = await producer.ProduceAsync("ativos.subscricao", testMessage);

                _logger.LogInformation("✅ Teste Kafka bem-sucedido - Partition: {Partition}, Offset: {Offset}",
                    result.Partition.Value, result.Offset.Value);

                return Ok(ApiResponse<object>.CreateSuccess(
                    new { status = "ok", partition = result.Partition.Value, offset = result.Offset.Value },
                    "Conexão com Kafka OK"
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro ao testar Kafka");
                return StatusCode(500, ApiResponse<object>.CreateError($"Erro na conexão com Kafka: {ex.Message}"));
            }
        }

        private async Task PublishSubscriptionEvent(string ativo, string acao)
        {
            try
            {
                _logger.LogInformation("🚀 Iniciando PublishSubscriptionEvent - Ativo: {Ativo}, Ação: {Acao}", ativo, acao);

                var config = new ProducerConfig
                {
                    BootstrapServers = "localhost:9092",
                    MessageTimeoutMs = 5000,
                    RequestTimeoutMs = 5000,
                    DeliveryReportFields = "all"
                };

                _logger.LogInformation("📡 Criando producer Kafka...");
                using var producer = new ProducerBuilder<string, string>(config).Build();

                var evento = new AtivoSubscricaoEvent
                {
                    Ativo = ativo.ToUpper(),
                    Acao = acao,
                    Timestamp = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(evento);
                _logger.LogInformation("📤 Evento serializado: {EventoJson}", json);

                var message = new Message<string, string>
                {
                    Key = ativo,
                    Value = json
                };

                _logger.LogInformation("📨 Enviando mensagem para tópico 'ativos.subscricao'...");
                var result = await producer.ProduceAsync("ativos.subscricao", message);

                _logger.LogInformation("✅ Evento enviado com sucesso! Ativo: {Ativo}, Ação: {Acao}, Partition: {Partition}, Offset: {Offset}",
                    ativo, acao, result.Partition.Value, result.Offset.Value);
            }
            catch (ProduceException<string, string> ex)
            {
                _logger.LogError(ex, "❌ Erro específico do Kafka Producer - Ativo: {Ativo}, Ação: {Acao}, Error Code: {ErrorCode}",
                    ativo, acao, ex.Error.Code);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erro geral ao enviar evento - Ativo: {Ativo}, Ação: {Acao}", ativo, acao);
                throw;
            }
        }
    }
}
