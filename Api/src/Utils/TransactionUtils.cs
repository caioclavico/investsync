using System.Collections.Generic;
using System.Linq;
using InvestSync.Api.src.Models;
using InvestSync.Api.src.DTOs;

namespace InvestSync.Api.src.Utils
{
    public static class TransactionUtils
    {
        public static List<TransactionResponse> MapWithRunningBalance(IEnumerable<Transaction> transactions)
        {
            decimal runningBalance = 0;
            var ordered = transactions.OrderBy(t => t.Date);
            var response = new List<TransactionResponse>();

            foreach (var t in ordered)
            {
                runningBalance += t.Type == TransactionType.Deposit ? t.Amount : -t.Amount;
                response.Add(new TransactionResponse
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Date = t.Date,
                    Type = t.Type.ToString(),
                    Description = t.Description,
                    Balance = runningBalance,
                    Message = ""
                });
            }
            return response;
        }
    }
}