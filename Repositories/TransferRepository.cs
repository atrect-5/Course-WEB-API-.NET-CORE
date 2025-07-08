﻿using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repositories
{
    public class TransferRepository(ProjectDBContext context) : ITransferService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        private const int TransferReceivedCategoryId = 1;
        private const int TransferSentCategoryId = 2;

        public bool Add(Transfer model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (model.MoneyAccountSendId == model.MoneyAccountReceiveId)
                throw new ArgumentException("La cuenta de origen y destino no pueden ser la misma.", nameof(model));

            var sendingAccount = _dbContext.MoneyAccounts.Find(model.MoneyAccountSendId)
                ?? throw new ArgumentException($"La cuenta de origen con ID {model.MoneyAccountSendId} no existe.", nameof(model));

            var receivingAccount = _dbContext.MoneyAccounts.Find(model.MoneyAccountReceiveId)
                ?? throw new ArgumentException($"La cuenta de destino con ID {model.MoneyAccountReceiveId} no existe.", nameof(model));

            if (sendingAccount.AccountType != "CREDIT" && sendingAccount.Balance < model.Amount)
                throw new InvalidOperationException("La cuenta de origen no tiene fondos suficientes para realizar la transferencia.");

            // Aplicar cambios de saldo, considerando el tipo de cuenta
            // Si se envía desde una cuenta de crédito, la deuda (balance) aumenta.
            // Si se envía desde otra cuenta, el saldo disminuye.
            sendingAccount.Balance += sendingAccount.AccountType == "CREDIT" ? model.Amount : -model.Amount;

            // Si se recibe en una cuenta de crédito, es un pago, la deuda (balance) disminuye.
            // Si se recibe en otra cuenta, el saldo aumenta.
            receivingAccount.Balance += receivingAccount.AccountType == "CREDIT" ? -model.Amount : model.Amount;

            // Validar que el límite de crédito no se exceda
            if (sendingAccount.AccountType == "CREDIT" && sendingAccount.Balance > sendingAccount.CreditLimit)
                throw new InvalidOperationException("La transferencia excede el límite de crédito de la cuenta de origen.");


            // Crear las dos transacciones asociadas
            var expenditureTransaction = CreateAssociatedTransaction(model, TransferSentCategoryId, model.MoneyAccountSendId);
            var incomeTransaction = CreateAssociatedTransaction(model, TransferReceivedCategoryId, model.MoneyAccountReceiveId);

            _dbContext.Transfers.Add(model);
            _dbContext.Transactions.Add(expenditureTransaction);
            _dbContext.Transactions.Add(incomeTransaction);

            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            // Incluimos las transacciones para borrarlas en cascada
            var transfer = _dbContext.Transfers
                .Include(t => t.Transactions)
                .FirstOrDefault(t => t.Id == id);

            if (transfer is null)
                return false;

            // Revertir cambios de saldo 
            var sendingAccount = _dbContext.MoneyAccounts.Find(transfer.MoneyAccountSendId);
            var receivingAccount = _dbContext.MoneyAccounts.Find(transfer.MoneyAccountReceiveId);

            if (sendingAccount is not null)
            {
                sendingAccount.Balance += sendingAccount.AccountType == "CREDIT" ? -transfer.Amount : transfer.Amount;
            }
            if (receivingAccount is not null)
            {
                receivingAccount.Balance += receivingAccount.AccountType == "CREDIT" ? transfer.Amount : -transfer.Amount;
            }

            // Eliminar las transacciones asociadas y la transferencia
            _dbContext.Transactions.RemoveRange(transfer.Transactions);
            _dbContext.Transfers.Remove(transfer);

            _dbContext.SaveChanges();
            return true;
        }

        public Transfer? GetTransferById(int id) =>
            _dbContext.Transfers.Find(id);

        public IEnumerable<Transfer> GetTransfersByUserId(int userId, int? moneyAccountId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbContext.Transfers.Where(t => t.UserId == userId);

            if (moneyAccountId.HasValue)
                query = query.Where(t => t.MoneyAccountSendId == moneyAccountId.Value || t.MoneyAccountReceiveId == moneyAccountId.Value);
            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            return query.OrderByDescending(t => t.Date).ToList();
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
    }
}
