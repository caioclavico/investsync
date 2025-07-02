using System.Text.Json;
using Confluent.Kafka;
using Shared.Events;
using Microsoft.Extensions.Logging;

namespace InvestSync.Api.src.Utils
{

    public static class PublishEvents
    {
        public static async Task<DeliveryResult<string, string>> PublishSubscriptionEvent(
            string ativo,
            string acao,
            string topic,
            IProducer<string, string> producer,
            ILogger? logger = null)
        {
            try
            {
                logger?.LogInformation("🚀 PublishEvents - Criando evento para {Ativo}, ação: {Acao}", ativo, acao);

                var evento = new AtivoSubscricaoEvent
                {
                    Ativo = ativo.ToUpper(),
                    Acao = acao,
                    Timestamp = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(evento);
                logger?.LogInformation("📤 PublishEvents - Evento serializado: {EventoJson}", json);

                var message = new Message<string, string>
                {
                    Key = ativo,
                    Value = json
                };

                logger?.LogInformation("📨 PublishEvents - Enviando para tópico: {Topic}", topic);
                var result = await producer.ProduceAsync(topic, message);

                logger?.LogInformation("✅ PublishEvents - Sucesso! Partition: {Partition}, Offset: {Offset}",
                    result.Partition.Value, result.Offset.Value);

                return result;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "❌ PublishEvents - Erro ao enviar evento: {Ativo}, {Acao}", ativo, acao);
                throw;
            }
        }
    }
}