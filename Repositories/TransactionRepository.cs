using Data;
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
        public bool Add(Transaction model)
        {
            ArgumentNullException.ThrowIfNull(model);

            // Validamos que las entidades relacionadas existan.
            var account = _dbContext.MoneyAccounts.Find(model.MoneyAccountId) 
                ?? throw new ArgumentException($"La cuenta con ID {model.MoneyAccountId} no existe.", nameof(model));
            var category = _dbContext.Categories.Find(model.CategoryId) 
                ?? throw new ArgumentException($"La categoría con ID {model.CategoryId} no existe.", nameof(model));

            ApplyTransactionBalanceChange(account, category, model.Amount);

            _dbContext.Transactions.Add(model);
            _dbContext.SaveChanges();

            return true;
        }

        public bool Delete(int id)
        {
            var transaction = _dbContext.Transactions.Find(id);
            if (transaction is null)
            {
                return false;
            }

            PreventDirectModificationOfTransferTransaction(transaction);

            var account = _dbContext.MoneyAccounts.Find(transaction.MoneyAccountId);
            var category = _dbContext.Categories.Find(transaction.CategoryId);

            RevertTransactionBalanceChange(account, category, transaction.Amount);

            _dbContext.Transactions.Remove(transaction);
            _dbContext.SaveChanges();
            return true;
        }

        public Transaction? GetTransactionById(int id) =>
            _dbContext.Transactions.Find(id);

        public IEnumerable<Transaction> GetTransactionsByUserId(
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

            return query.OrderByDescending(t => t.Date).ToList();
        }

        public Transaction? Update(Transaction model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var transactionInDb = _dbContext.Transactions.Find(model.Id);
            if (transactionInDb is null)
            {
                return null;
            }

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
            return transactionInDb;
        }

        /// <summary>
        /// Applies the financial impact of a transaction to a money account's balance.
        /// </summary>
        private static void ApplyTransactionBalanceChange(MoneyAccount account, Category category, decimal amount)
        {
            if (category.Type == "INCOME") account.Balance += amount;
            else account.Balance -= amount; // Assumes EXPENDITURE
        }

        /// <summary>
        /// Reverts the financial impact of a transaction from a money account's balance.
        /// </summary>
        private static void RevertTransactionBalanceChange(MoneyAccount? account, Category? category, decimal amount)
        {
            if (account is not null && category is not null)
            {
                if (category.Type == "INCOME") account.Balance -= amount;
                else account.Balance += amount; // Assumes EXPENDITURE
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
            {
                throw new InvalidOperationException("Las transacciones que forman parte de una transferencia no se pueden modificar o eliminar directamente. Opere sobre la transferencia original.");
            }
        }
    }
}
