﻿using Data;
using Dtos.Transaction;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class TransactionRepository(ProjectDBContext context) : ITransactionService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        public TransactionDto Add(CreateTransactionDto model)
        {
            ArgumentNullException.ThrowIfNull(model);
            var transaction = new Transaction
            {
                Amount = model.Amount,
                Date = model.Date,
                Description = model.Description,
                UserId = model.UserId,
                CategoryId = model.CategoryId,
                MoneyAccountId = model.MoneyAccountId
            };

            // Validamos que las entidades relacionadas existan.
            var account = _dbContext.MoneyAccounts.Find(model.MoneyAccountId)
                ?? throw new ArgumentException($"La cuenta con ID {model.MoneyAccountId} no existe.", nameof(model));
            var category = _dbContext.Categories.Find(model.CategoryId)
                ?? throw new ArgumentException($"La categoría con ID {model.CategoryId} no existe.", nameof(model));

            ApplyTransactionBalanceChange(account, category, transaction.Amount);

            transaction.Date ??= DateTime.Now;

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return MapToDto(transaction);
        }

        public bool Delete(int id)
        {
            var transaction = _dbContext.Transactions.Find(id);
            if (transaction is null)
                return false;

            PreventDirectModificationOfTransferTransaction(transaction);

            var account = _dbContext.MoneyAccounts.Find(transaction.MoneyAccountId);
            var category = _dbContext.Categories.Find(transaction.CategoryId);

            RevertTransactionBalanceChange(account, category, transaction.Amount);

            _dbContext.Transactions.Remove(transaction);
            _dbContext.SaveChanges();
            return true;
        }

        public TransactionDto? GetTransactionById(int id)
        {
            var transaction = _dbContext.Transactions.Find(id);
            if (transaction is null) return null;
            return MapToDto(transaction);
        }

        public IEnumerable<TransactionDto> GetTransactionsByUserId(
            int userId, int? moneyAccountId = null, int? categoryId = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            // Empezamos filtrando por el usuario.
            var query = _dbContext.Transactions.Where(t => t.UserId == userId);

            // Aplicamos los filtros opcionales de forma condicional.
            if (moneyAccountId.HasValue)
                query = query.Where(t => t.MoneyAccountId == moneyAccountId.Value);
            if (categoryId.HasValue)
                query = query.Where(t => t.CategoryId == categoryId.Value);
            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            return query.OrderByDescending(t => t.Date).Select(t => MapToDto(t)).ToList();
        }

        public TransactionDto? Update(int id, UpdateTransactionDto model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var transactionInDb = _dbContext.Transactions.Find(id);
            if (transactionInDb is null)
                return null;

            PreventDirectModificationOfTransferTransaction(transactionInDb);

            // Revertir el impacto de la transacción original en el saldo
            var originalAccount = _dbContext.MoneyAccounts.Find(transactionInDb.MoneyAccountId);
            var originalCategory = _dbContext.Categories.Find(transactionInDb.CategoryId);
            RevertTransactionBalanceChange(originalAccount, originalCategory, transactionInDb.Amount);

            // Aplicar el impacto de la nueva transacción en el saldo
            var newAccount = _dbContext.MoneyAccounts.Find(model.MoneyAccountId)
                ?? throw new ArgumentException($"La nueva cuenta con ID {model.MoneyAccountId} no existe.", nameof(model));
            var newCategory = _dbContext.Categories.Find(model.CategoryId)
                ?? throw new ArgumentException($"La nueva categoría con ID {model.CategoryId} no existe.", nameof(model));
            ApplyTransactionBalanceChange(newAccount, newCategory, model.Amount);

            // Actualizar los datos de la transacción en la base de datos
            transactionInDb.Amount = model.Amount;
            transactionInDb.Date = model.Date;
            transactionInDb.Description = model.Description;
            transactionInDb.CategoryId = model.CategoryId;
            transactionInDb.MoneyAccountId = model.MoneyAccountId;

            _dbContext.SaveChanges();
            return MapToDto(transactionInDb);
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
        
        // <summary>
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
