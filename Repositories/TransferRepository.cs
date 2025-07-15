﻿using Common;
using Data;
using Dtos.Transaction;
using Dtos.Transfer;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

namespace Repositories
{
    public class TransferRepository(ProjectDBContext context) : ITransferService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        private const string TransferReceivedCategoryName = "TRANSFER RECEIVED";
        private const string TransferSentCategoryName = "TRANSFER SENT";

        public async Task<OperationResult<TransferDto>> AddAsync(CreateTransferDto model, int userId, bool isAdmin)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (model.MoneyAccountSendId == model.MoneyAccountReceiveId)
                throw new ArgumentException("La cuenta de origen y destino no pueden ser la misma.", nameof(model));

            var sendingAccount = await _dbContext.MoneyAccounts.FindAsync(model.MoneyAccountSendId);
            if (sendingAccount is null)
                return OperationResult<TransferDto>.Fail(Result.NotFound);
            if (sendingAccount.UserId != userId && !isAdmin)
                return OperationResult<TransferDto>.Fail(Result.Forbidden);

            var receivingAccount = await _dbContext.MoneyAccounts.FindAsync(model.MoneyAccountReceiveId);
            if (receivingAccount is null)
                return OperationResult<TransferDto>.Fail(Result.NotFound);
            if (receivingAccount.UserId != userId && !isAdmin)
                return OperationResult<TransferDto>.Fail(Result.Forbidden);

            if (receivingAccount.UserId != sendingAccount.UserId)
                throw new ArgumentException("La cuenta de origen y destino deben ser del mismo usuario.", nameof(model));

            var transfer = new Transfer
            {
                Amount = model.Amount,
                Date = model.Date ?? DateTime.Now,
                Description = model.Description,
                UserId = sendingAccount.UserId,
                MoneyAccountSendId = model.MoneyAccountSendId,
                MoneyAccountReceiveId = model.MoneyAccountReceiveId
            };

            if (sendingAccount.AccountType != "CREDIT" && sendingAccount.Balance < transfer.Amount)
                throw new InvalidOperationException("La cuenta de origen no tiene fondos suficientes para realizar la transferencia.");

            // Aplicar cambios de saldo, considerando el tipo de cuenta
            // Si se envía desde una cuenta de crédito, la deuda (balance) aumenta.
            // Si se envía desde otra cuenta, el saldo disminuye.
            sendingAccount.Balance += sendingAccount.AccountType == "CREDIT" ? transfer.Amount : -transfer.Amount;

            // Si se recibe en una cuenta de crédito, es un pago, la deuda (balance) disminuye.
            // Si se recibe en otra cuenta, el saldo aumenta.
            receivingAccount.Balance += receivingAccount.AccountType == "CREDIT" ? -transfer.Amount : transfer.Amount;

            // Validar que el límite de crédito no se exceda
            if (sendingAccount.AccountType == "CREDIT" && sendingAccount.Balance > sendingAccount.CreditLimit)
                throw new InvalidOperationException("La transferencia excede el límite de crédito de la cuenta de origen.");

            var receivedCategory = await _dbContext.Categories.FirstOrDefaultAsync(c => c.UserId == null && c.Name.ToUpper() == TransferReceivedCategoryName)
                ?? throw new InvalidOperationException($"La categoría global '{TransferReceivedCategoryName}' no se encuentra en la base de datos. Asegúrate de que exista, que su tipo sea 'INCOME' y que no tenga un UserId asignado.");
            var sentCategory = await _dbContext.Categories.FirstOrDefaultAsync(c => c.UserId == null && c.Name.ToUpper() == TransferSentCategoryName)
                ?? throw new InvalidOperationException($"La categoría global '{TransferSentCategoryName}' no se encuentra en la base de datos. Asegúrate de que exista, que su tipo sea 'EXPENDITURE' y que no tenga un UserId asignado.");
            // Crear las dos transacciones asociadas
            var expenditureTransaction = CreateAssociatedTransaction(transfer, sentCategory.Id, transfer.MoneyAccountSendId);
            var incomeTransaction = CreateAssociatedTransaction(transfer, receivedCategory.Id, transfer.MoneyAccountReceiveId);

            await _dbContext.Transfers.AddAsync(transfer);
            await _dbContext.Transactions.AddAsync(expenditureTransaction);
            await _dbContext.Transactions.AddAsync(incomeTransaction);

            await _dbContext.SaveChangesAsync();
            return OperationResult<TransferDto>.Success(MapToDto(transfer));
        }

        public async Task<OperationResult<bool>> DeleteAsync(int id, int userId, bool isAdmin)
        {
            // Incluimos las transacciones para borrarlas en cascada
            var transfer = await _dbContext.Transfers
                .Include(t => t.Transactions)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transfer is null)
                return OperationResult<bool>.Fail(Result.NotFound);
            if (transfer.UserId != userId && !isAdmin)
                return OperationResult<bool>.Fail(Result.Forbidden);

            // Revertir cambios de saldo 
            var sendingAccount = await _dbContext.MoneyAccounts.FindAsync(transfer.MoneyAccountSendId);
            var receivingAccount = await _dbContext.MoneyAccounts.FindAsync(transfer.MoneyAccountReceiveId);

            if (sendingAccount is not null)
                sendingAccount.Balance += sendingAccount.AccountType == "CREDIT" ? -transfer.Amount : transfer.Amount;
            
            if (receivingAccount is not null)
                receivingAccount.Balance += receivingAccount.AccountType == "CREDIT" ? transfer.Amount : -transfer.Amount;
            
            // Eliminar las transacciones asociadas y la transferencia
            _dbContext.Transactions.RemoveRange(transfer.Transactions);
            _dbContext.Transfers.Remove(transfer);

            await _dbContext.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }

        public async Task<TransferDto?> GetTransferByIdAsync(int id)
        {
            var transfer = await _dbContext.Transfers
                .Include(t => t.Transactions)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transfer is null) return null;
            return MapToDto(transfer);
        }

        public async Task<IEnumerable<TransferDto>> GetTransfersByUserIdAsync(int userId, int? moneyAccountId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.Transfers.Where(t => t.UserId == userId);

            if (moneyAccountId.HasValue)
                query = query.Where(t => t.MoneyAccountSendId == moneyAccountId.Value || t.MoneyAccountReceiveId == moneyAccountId.Value);
            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

             return await query.Include(t => t.Transactions)
                        .OrderByDescending(t => t.Date)
                        .Select(t => MapToDto(t))
                        .ToListAsync();
        }

        /// <summary>
        /// Creates a transaction record associated with a transfer.
        /// </summary>
        private static Transaction CreateAssociatedTransaction(Transfer transfer, int categoryId, int accountId)
        {
            return new Transaction
            {
                Amount = transfer.Amount,
                Date = transfer.Date,
                Description = $"Transferencia: {transfer.Description}",
                UserId = transfer.UserId,
                CategoryId = categoryId,
                MoneyAccountId = accountId,
                Transfer = transfer
            };
        }

        /// <summary>
        /// Maps a <see cref="Transfer"/> entity to a <see cref="TransferDto"/>.
        /// </summary>
        /// <param name="transfer">The transfer entity to map.</param>
        /// <returns>A new <see cref="TransferDto"/> instance.</returns>
        private static TransferDto MapToDto(Transfer transfer)
        {
            return new TransferDto
            {
                Id = transfer.Id,
                Amount = transfer.Amount,
                Date = transfer.Date,
                Description = transfer.Description,
                UserId = transfer.UserId,
                MoneyAccountSendId = transfer.MoneyAccountSendId,
                MoneyAccountReceiveId = transfer.MoneyAccountReceiveId,
                Transactions = transfer.Transactions?.Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Date = t.Date,
                    Description = t.Description,
                    UserId = t.UserId,
                    CategoryId = t.CategoryId,
                    MoneyAccountId = t.MoneyAccountId,
                    TransferId = t.TransferId
                }).ToList() ?? new List<TransactionDto>()
            };
        }
    }
}
