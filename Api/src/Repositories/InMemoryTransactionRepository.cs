using System.Collections.Concurrent;
using InvestSync.Api.src.Models;
using InvestSync.Api.src.Interfaces;

namespace InvestSync.Api.src.Repositories
{
    public class InMemoryTransactionRepository : ITransactionRepository
    {
        private static readonly ConcurrentDictionary<Guid, List<Transaction>> _transactions = new();

        public Task<decimal> GetBalanceAsync(Guid userId)
        {
            var transactions = _transactions.GetValueOrDefault(userId) ?? new List<Transaction>();
            var balance = transactions.Sum(t => t.Type == TransactionType.Deposit ? t.Amount : -t.Amount);
            return Task.FromResult(balance);
        }

        public Task DepositAsync(Guid userId, decimal amount, string description = "")
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = amount,
                Type = TransactionType.Deposit,
                Description = description,
                Date = DateTime.UtcNow
            };

            var list = _transactions.GetOrAdd(userId, _ => new List<Transaction>());
            lock (list)
            {
                list.Add(transaction);
            }
            return Task.CompletedTask;
        }

        public Task<bool> WithdrawAsync(Guid userId, decimal amount, string description = "")
        {

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = amount,
                Type = TransactionType.Withdraw,
                Description = description,
                Date = DateTime.UtcNow
            };

            var list = _transactions.GetOrAdd(userId, _ => new List<Transaction>());
            lock (list)
            {
                list.Add(transaction);
            }
            return Task.FromResult(true);
        }

        public Task<IEnumerable<Transaction>> GetTransactionsAsync(Guid userId)
        {
            var transactions = _transactions.GetValueOrDefault(userId) ?? new List<Transaction>();
            return Task.FromResult(transactions.AsEnumerable());
        }
    }
}