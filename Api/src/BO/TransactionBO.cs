using InvestSync.Api.src.Interfaces;

namespace InvestSync.Api.src.BO
{
    public class TransactionBO
    {
        public static void ValidateTransactionDeposit(DTOs.TransactionRequest request)
        {
            if (request.Amount <= 0)
            {
                throw new ArgumentException("O valor deve ser positivo.");
            }
        }

        public static void ValidateTransactionWithdrawal(DTOs.TransactionRequest request, decimal balance)
        {
            if (request.Amount <= 0)
            {
                throw new ArgumentException("O valor deve ser positivo.");
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentException("A descrição não pode estar vazia.");
            }
            if (request.Amount > balance)
            {
                throw new InvalidOperationException("Saldo insuficiente.");
            }
        }


    }
}