using Confluent.Kafka;
using System.Text.Json;
using Shared.Events;

namespace InvestSync.Worker.Utils;

public class AtivoSubscriptionPublisher
{
    private readonly IProducer<string, string> _producer;

    public AtivoSubscriptionPublisher(IProducer<string, string> producer)
    {
        _producer = producer;
    }

    public async Task PublishSubscribeEvent(string ativo)
    {
        var evento = new AtivoSubscricaoEvent
        {
            Ativo = ativo,
            Acao = "subscribe",
            Timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(evento);

        await _producer.ProduceAsync("ativos.subscricao", new Message<string, string>
        {
            Key = ativo,
            Value = json
        });
    }

    public async Task PublishUnsubscribeEvent(string ativo)
    {
        var evento = new AtivoSubscricaoEvent
        {
            Ativo = ativo,
            Acao = "unsubscribe",
            Timestamp = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(evento);

        await _producer.ProduceAsync("ativos.subscricao", new Message<string, string>
        {
            Key = ativo,
            Value = json
        });
    }
}
