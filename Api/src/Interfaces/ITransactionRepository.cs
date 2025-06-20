using InvestSync.Api.src.Models;

namespace InvestSync.Api.src.Interfaces
{
    public interface ITransactionRepository
    {
        Task<decimal> GetBalanceAsync(Guid userId);
        Task DepositAsync(Guid userId, decimal amount, string description = "");
        Task<bool> WithdrawAsync(Guid userId, decimal amount, string description = "");
        Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid userId);
    }
}