﻿using Common;
using Data;
using Dtos.Transaction;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

namespace Repositories
{
    public class TransactionRepository(ProjectDBContext context) : ITransactionService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<OperationResult<TransactionDto>> AddAsync(CreateTransactionDto model, int userId, bool isAdmin)
        {
            ArgumentNullException.ThrowIfNull(model);

            // Validamos que las entidades relacionadas existan y que el usuario tenga permiso sobre ellas.
            var account = await _dbContext.MoneyAccounts.FindAsync(model.MoneyAccountId);
            if (account is null)
                return OperationResult<TransactionDto>.Fail(Result.NotFound);
            if (account.UserId != userId && !isAdmin)
                return OperationResult<TransactionDto>.Fail(Result.Forbidden);

            var category = await _dbContext.Categories.FindAsync(model.CategoryId);
            if (category is null)
                return OperationResult<TransactionDto>.Fail(Result.NotFound);
            if (category.UserId is not null && category.UserId != userId && !isAdmin)
                return OperationResult<TransactionDto>.Fail(Result.Forbidden);

            var transaction = new Transaction
            {
                Amount = model.Amount,
                Date = model.Date,
                Description = model.Description,
                UserId = account.UserId,
                CategoryId = model.CategoryId,
                MoneyAccountId = model.MoneyAccountId
            };

            ApplyTransactionBalanceChange(account, category, transaction.Amount);

            transaction.Date ??= DateTime.Now;

            await _dbContext.Transactions.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();

            return OperationResult<TransactionDto>.Success(MapToDto(transaction));
        }

        public async Task<OperationResult<bool>> DeleteAsync(int id, int userId, bool isAdmin)
        {
            var transaction = await _dbContext.Transactions.FindAsync(id);
            if (transaction is null)
                return OperationResult<bool>.Fail(Result.NotFound);

            if (transaction.UserId != userId && !isAdmin)
                return OperationResult<bool>.Fail(Result.Forbidden);

            PreventDirectModificationOfTransferTransaction(transaction);

            var account = await _dbContext.MoneyAccounts.FindAsync(transaction.MoneyAccountId);
            var category = await _dbContext.Categories.FindAsync(transaction.CategoryId);

            if (account is not null && category is not null)
                RevertTransactionBalanceChange(account, category, transaction.Amount);

            _dbContext.Transactions.Remove(transaction);
            await _dbContext.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _dbContext.Transactions.FindAsync(id);
            if (transaction is null) return null;
            return MapToDto(transaction);
        }

        public async Task<IEnumerable<TransactionDto>> GetTransactionsByUserIdAsync(
            int userId, int? moneyAccountId = null, int? categoryId = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.Transactions.Where(t => t.UserId == userId);

            if (moneyAccountId.HasValue)
                query = query.Where(t => t.MoneyAccountId == moneyAccountId.Value);
            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);
            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            var result = await query.OrderByDescending(t => t.Date).Select(t => MapToDto(t)).ToListAsync();
            return result;
        }

        public async Task<OperationResult<TransactionDto>> UpdateAsync(int id, UpdateTransactionDto model, int userId, bool isAdmin)
        {
            ArgumentNullException.ThrowIfNull(model);

            var transactionInDb = await _dbContext.Transactions.FindAsync(id);
            if (transactionInDb is null)
                return OperationResult<TransactionDto>.Fail(Result.NotFound);

            if (transactionInDb.UserId != userId && !isAdmin)
                return OperationResult<TransactionDto>.Fail(Result.Forbidden);

            PreventDirectModificationOfTransferTransaction(transactionInDb);

            // Revertir el impacto de la transacción original en el saldo
            var originalAccount = await _dbContext.MoneyAccounts.FindAsync(transactionInDb.MoneyAccountId);
            var originalCategory = await _dbContext.Categories.FindAsync(transactionInDb.CategoryId);
            RevertTransactionBalanceChange(originalAccount, originalCategory, transactionInDb.Amount);

            // Aplicar el impacto de la nueva transacción en el saldo
            var newAccount = await _dbContext.MoneyAccounts.FindAsync(model.MoneyAccountId);
            if (newAccount is null)
                return OperationResult<TransactionDto>.Fail(Result.NotFound);
            if (newAccount.UserId != userId && !isAdmin)
                return OperationResult<TransactionDto>.Fail(Result.Forbidden);

            var newCategory = await _dbContext.Categories.FindAsync(model.CategoryId);
            if (newCategory is null)
                return OperationResult<TransactionDto>.Fail(Result.NotFound);
            if (newCategory.UserId is not null && newCategory.UserId != userId && !isAdmin)
                return OperationResult<TransactionDto>.Fail(Result.Forbidden);
            ApplyTransactionBalanceChange(newAccount, newCategory, model.Amount);

            // Actualizar los datos de la transacción en la base de datos
            transactionInDb.Amount = model.Amount;
            transactionInDb.Date = model.Date;
            transactionInDb.Description = model.Description;
            transactionInDb.CategoryId = model.CategoryId;
            transactionInDb.MoneyAccountId = model.MoneyAccountId;

            await _dbContext.SaveChangesAsync();
            return OperationResult<TransactionDto>.Success(MapToDto(transactionInDb));
        }

        /// <summary>
        /// Applies the financial impact of a transaction to a money account's balance.
        /// </summary>
        private static void ApplyTransactionBalanceChange(MoneyAccount account, Category category, decimal amount)
        {
            bool isCreditAccount = account.AccountType == "CREDIT";

            if (category.Type == "INCOME")
                // Un ingreso en una cuenta de crédito reduce la deuda (el balance).
                // En cualquier otra cuenta, aumenta el saldo.
                account.Balance += isCreditAccount ? -amount : amount;
            else
                // Un gasto en una cuenta de crédito aumenta la deuda (el balance).
                // En cualquier otra cuenta, reduce el saldo.
                account.Balance += isCreditAccount ? amount : -amount;
        }

        /// <summary>
        /// Reverts the financial impact of a transaction from a money account's balance.
        /// </summary>
        private static void RevertTransactionBalanceChange(MoneyAccount? account, Category? category, decimal amount)
        {
            if (account is not null && category is not null)
            {
                bool isCreditAccount = account.AccountType == "CREDIT";

                if (category.Type == "INCOME")
                    // Revertir un ingreso en una cuenta de crédito aumenta la deuda (el balance).
                    // En cualquier otra cuenta, reduce el saldo.
                    account.Balance += isCreditAccount ? amount : -amount;
                else
                    // Revertir un gasto en una cuenta de crédito reduce la deuda (el balance).
                    // En cualquier otra cuenta, aumenta el saldo.
                    account.Balance += isCreditAccount ? -amount : amount;
            }
        }

        /// <summary>
        /// Ensures that a transaction is not part of a transfer, preventing direct modification or deletion.
        /// </summary>
        /// <param name="transaction">The transaction to check.</param>
        /// <exception cref="InvalidOperationException">Thrown if the transaction is linked to a transfer.</exception>
        private static void PreventDirectModificationOfTransferTransaction(Transaction transaction)
        {
            if (transaction.TransferId is not null)
                throw new InvalidOperationException("Las transacciones que forman parte de una transferencia no se pueden modificar o eliminar directamente. Opere sobre la transferencia original.");
        }
        
        /// <summary>
        /// Maps a <see cref="Transaction"/> entity to a <see cref="TransactionDto"/>.
        /// </summary>
        /// <param name="transaction">The transaction entity to map.</param>
        /// <returns>A new <see cref="TransactionDto"/> instance.</returns>
        private static TransactionDto MapToDto(Transaction transaction)
        {
            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Date = transaction.Date,
                Description = transaction.Description,
                UserId = transaction.UserId,
                CategoryId = transaction.CategoryId,
                MoneyAccountId = transaction.MoneyAccountId,
                TransferId = transaction.TransferId
            };
        }
    }
}
