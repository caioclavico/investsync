namespace Shared.Events;

public class PrecoAtualizadoEvent
{
    public string Ativo { get; set; }
    public decimal Preco { get; set; }
    public DateTime Timestamp { get; set; }

    public PrecoAtualizadoEvent()
    {
        Ativo = string.Empty;
    }
}