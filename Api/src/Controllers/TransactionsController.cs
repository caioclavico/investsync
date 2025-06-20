using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using InvestSync.Api.src.DTOs;
using InvestSync.Api.src.Interfaces;
using InvestSync.Api.src.BO;
using InvestSync.Api.src.Utils;

namespace InvestSync.Api.src.Controllers
{
    [ApiController]
    [Route("transactions")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionsController(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var guid) ? guid : Guid.Empty;
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> Deposit([FromBody] TransactionRequest request)
        {
            try
            {
                TransactionBO.ValidateTransactionDeposit(request);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TransactionResponse>.CreateError(ex.Message));
            }

            var userId = GetUserId();
            await _transactionRepository.DepositAsync(userId, request.Amount, request.Description);

            var transactions = await _transactionRepository.GetTransactionsAsync(userId);
            var lastTransaction = transactions.LastOrDefault();
            var balance = await _transactionRepository.GetBalanceAsync(userId);

            var response = TransactionResponse.Create(lastTransaction, balance, $"Depósito de {request.Amount:C} realizado com sucesso.");

            return Ok(ApiResponse<TransactionResponse>.CreateSuccess(response, "Transação realizada com sucesso."));
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] TransactionRequest request)
        {
            var userId = GetUserId();
            var balance = await _transactionRepository.GetBalanceAsync(userId);

            try
            {
                TransactionBO.ValidateTransactionWithdrawal(request, balance);
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse<TransactionResponse>.CreateError(ex.Message));
            }

            await _transactionRepository.WithdrawAsync(userId, request.Amount, request.Description);

            var transactions = await _transactionRepository.GetTransactionsAsync(userId);
            var lastTransaction = transactions.LastOrDefault();
            var newBalance = await _transactionRepository.GetBalanceAsync(userId);

            var response = TransactionResponse.Create(lastTransaction, newBalance, $"Saque de {request.Amount:C} realizado com sucesso.");

            return Ok(ApiResponse<TransactionResponse>.CreateSuccess(response, "Transação realizada com sucesso."));
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            var userId = GetUserId();
            var transactions = await _transactionRepository.GetTransactionsAsync(userId);

            var responseList = TransactionUtils.MapWithRunningBalance(transactions);
            var finalBalance = responseList.LastOrDefault()?.Balance ?? 0;

            var statement = new TransactionStatementResponse
            {
                Transactions = responseList,
                Balance = finalBalance
            };

            return Ok(ApiResponse<TransactionStatementResponse>.CreateSuccess(statement, "Extrato gerado com sucesso."));
        }
    }
}