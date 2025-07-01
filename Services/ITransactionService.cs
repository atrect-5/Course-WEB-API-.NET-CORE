using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITransactionService
    {
        /// <summary>
        /// Adds a new transaction to the system.
        /// </summary>
        /// <param name="model">The transaction to add. Cannot be null.</param>
        /// <returns><see langword="true"/> if the transaction was successfully created; otherwise, <see langword="false"/></returns>
        bool Add(Transaction model);

        /// <summary>
        /// Retrieves a transaction by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the transaction to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="Transaction"/> object corresponding to the specified identifier, or <see langword="null"/> if no transaction with the given identifier exists.</returns>
        Transaction? GetTransactionById(int id);

        /// <summary>
        /// Retrieves a collection of transactions for a specific user, with optional filters.
        /// </summary>
        /// <param name="userId">The ID of the user whose transactions are to be retrieved.</param>
        /// <param name="moneyAccountId">Optional. The ID of the money account to filter by.</param>
        /// <param name="categoryId">Optional. The ID of the category to filter by.</param>
        /// <param name="startDate">Optional. The start date of the date range filter.</param>
        /// <param name="endDate">Optional. The end date of the date range filter.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Transaction"/> objects matching the criteria.</returns>
        IEnumerable<Transaction> GetTransactionsByUserId(
            int userId, int? moneyAccountId = null, int? categoryId = null,
            DateTime? startDate = null, DateTime? endDate = null
        );

        /// <summary>
        /// Updates an existing transaction with new information.
        /// </summary>
        /// <param name="model">The updated transaction model. Must not be null and must include a valid identifier.</param>
        /// <returns>The updated <see cref="Transaction"/> object reflecting the changes, or <see langword="null"/> if the update operation fails or the transaction does not exist.</returns>
        Transaction? Update(Transaction model);

        /// <summary>
        /// Deletes the specified transaction from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the transaction to delete. Must be a positive integer.</param>
        /// <returns><see langword="true"/> if the transaction was successfully deleted; otherwise, <see langword="false"/>.</returns>
        bool Delete(int id);
    }
}
