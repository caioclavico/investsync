using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Confluent.Kafka;
using InvestSync.Api.src.DTOs;
using InvestSync.Api.src.Utils;

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

                _logger.LogInformation("📞 Chamando PublishEvents.PublishSubscriptionEvent...");

                var config = new ProducerConfig
                {
                    BootstrapServers = "localhost:9092",
                    MessageTimeoutMs = 5000,
                    RequestTimeoutMs = 5000,
                    DeliveryReportFields = "all"
                };

                using var producer = new ProducerBuilder<string, string>(config).Build();
                var result = await PublishEvents.PublishSubscriptionEvent(ativo, "subscribe", "ativos.subscricao", producer, _logger);

                _logger.LogInformation("✅ Evento de subscrição enviado via PublishEvents para {Ativo} - Partition: {Partition}, Offset: {Offset}",
                    ativo, result.Partition.Value, result.Offset.Value);

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

                _logger.LogInformation("📞 Chamando PublishEvents.PublishSubscriptionEvent...");

                var config = new ProducerConfig
                {
                    BootstrapServers = "localhost:9092",
                    MessageTimeoutMs = 5000,
                    RequestTimeoutMs = 5000,
                    DeliveryReportFields = "all"
                };

                using var producer = new ProducerBuilder<string, string>(config).Build();
                var result = await PublishEvents.PublishSubscriptionEvent(ativo, "unsubscribe", "ativos.subscricao", producer, _logger);

                _logger.LogInformation("✅ Evento de dessubscrição enviado via PublishEvents para {Ativo} - Partition: {Partition}, Offset: {Offset}",
                    ativo, result.Partition.Value, result.Offset.Value);

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
    }
}
