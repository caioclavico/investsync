using InvestSync.Api.src.Models;

namespace InvestSync.Api.src.DTOs
{
    public class TransactionResponse
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Message { get; set; } = string.Empty;

        public static TransactionResponse Create(Transaction? transaction, decimal balance, string message)
        {
            return new TransactionResponse
            {
                Id = transaction?.Id ?? Guid.Empty,
                Amount = transaction?.Amount ?? 0,
                Date = transaction?.Date ?? DateTime.MinValue,
                Type = transaction?.Type.ToString() ?? "",
                Description = transaction?.Description ?? "",
                Balance = balance,
                Message = message
            };
        }
    }
}