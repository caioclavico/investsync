namespace InvestSync.Api.src.DTOs
{
    public class TransactionStatementResponse
    {
        public IEnumerable<TransactionResponse> Transactions { get; set; } = new List<TransactionResponse>();
        public decimal Balance { get; set; }
    }
}