namespace InvestSync.Api.src.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public enum TransactionType
    {
        Deposit,
        Withdraw
    }
}