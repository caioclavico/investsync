namespace Shared.Events;

public class AtivoSubscricaoEvent
{
    public string Ativo { get; set; }
    public string Acao { get; set; } // "subscribe" ou "unsubscribe"
    public DateTime Timestamp { get; set; }

    public AtivoSubscricaoEvent()
    {
        Ativo = string.Empty;
        Acao = string.Empty;
    }
}
