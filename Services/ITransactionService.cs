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
        /// Retrieves a collection of all transactions associated with the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose transactions are to be retrieved. Must be a positive integer.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Transaction"/> objects representing the user's transactions. Returns an empty collection if no transactions are found.</returns>
        IEnumerable<Transaction> GetTransactionsByUserId(int userId);

        /// <summary>
        /// Retrieves a transaction by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the transaction to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="Transaction"/> object corresponding to the specified identifier, or <see langword="null"/> if no transaction with the given identifier exists.</returns>
        Transaction GetTransactionById(int id);

        /// <summary>
        /// Retrieves a collection of transactions associated with a specific category for a given user.
        /// </summary>
        /// <remarks>This method is useful for filtering transactions by category within the context of a
        /// specific user. Ensure that both <paramref name="userId"/> and <paramref name="categoryId"/> correspond to
        /// valid entities.</remarks>
        /// <param name="userId">The unique identifier of the user whose transactions are being retrieved. Must be a valid user ID.</param>
        /// <param name="categoryId">The unique identifier of the category for which transactions are being retrieved. Must be a valid category
        /// ID.</param>
        /// <returns>An <see cref="IEnumerable{Transaction}"/> containing the transactions that match the specified user and
        /// category. Returns an empty collection if no transactions are found.</returns>
        IEnumerable<Transaction> GetTransactionByCategoryId(int userId, int categoryId);

        /// <summary>
        /// Retrieves a collection of transactions associated with the specified money account ID.
        /// </summary>
        /// <param name="categoryId">The unique identifier of the money account for which transactions are to be retrieved.</param>
        /// <returns>An <see cref="IEnumerable{Transaction}"/> containing the transactions linked to the specified money account
        /// ID. If no transactions are found, the collection will be empty.</returns>
        IEnumerable<Transaction> GetTransactionByMoneyAccountId(int moneyAccountId);

        /// <summary>
        /// Retrieves a collection of transactions for a specified user within a given date range.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose transactions are to be retrieved. Must be a positive integer.</param>
        /// <param name="startDate">The start date of the date range. Transactions occurring on or after this date will be included.</param>
        /// <param name="endDate">The end date of the date range. Transactions occurring on or before this date will be included.</param>
        /// <returns>An enumerable collection of <see cref="Transaction"/> objects representing the user's transactions within
        /// the specified date range. Returns an empty collection if no transactions are found.</returns>
        IEnumerable<Transaction> GetTransactionByDateRange(int userId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Updates an existing transaction with new information.
        /// </summary>
        /// <param name="model">The updated transaction model. Must not be null and must include a valid identifier.</param>
        /// <returns>The updated <see cref="Transaction"/> object reflecting the changes, or <see langword="null"/> if the update operation fails or the transaction does not exist.</returns>
        Transaction Update(Transaction model);

        /// <summary>
        /// Deletes the specified transaction from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the transaction to delete. Must be a positive integer.</param>
        /// <returns><see langword="true"/> if the transaction was successfully deleted; otherwise, <see langword="false"/>.</returns>
        bool Delete(int id);
    }
}
