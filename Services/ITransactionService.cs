using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Dtos.Transaction;

namespace Services
{
    public interface ITransactionService
    {
        /// <summary>
        /// Adds a new transaction to the system.
        /// </summary>
        /// <param name="model">The DTO for creating the transaction. Cannot be null.</param>
        /// <param name="userId">The ID of the user creating the transaction.</param>
        /// <param name="isAdmin">A flag indicating if the user is an administrator.</param>
        /// <returns>A <see cref="TransactionDto"/> representing the newly created transaction.</returns>
        Task<OperationResult<TransactionDto>> AddAsync(CreateTransactionDto model, int userId, bool isAdmin);

        /// <summary>
        /// Retrieves a transaction by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the transaction to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="TransactionDto"/> object corresponding to the specified identifier, or <see langword="null"/> if no transaction with the given identifier exists.</returns>
        Task<TransactionDto?> GetTransactionByIdAsync(int id);

        /// <summary>
        /// Retrieves a collection of transactions for a specific user, with optional filters.
        /// </summary>
        /// <param name="userId">The ID of the user whose transactions are to be retrieved.</param>
        /// <param name="moneyAccountId">Optional. The ID of the money account to filter by.</param>
        /// <param name="categoryId">Optional. The ID of the category to filter by.</param>
        /// <param name="startDate">Optional. The start date of the date range filter.</param>
        /// <param name="endDate">Optional. The end date of the date range filter.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="TransactionDto"/> objects matching the criteria.</returns>
        Task<IEnumerable<TransactionDto>> GetTransactionsByUserIdAsync(
            int? userId, int? moneyAccountId = null, int? categoryId = null,
            DateTime? startDate = null, DateTime? endDate = null
        );

        /// <summary>
        /// Updates an existing transaction with new information.
        /// </summary>
        /// <param name="model">The DTO with the updated transaction information. Must not be null.</param>
        /// <param name="userId">The ID of the user performing the update.</param>
        /// <param name="isAdmin">A flag indicating if the user is an administrator.</param>
        /// <returns>An <see cref="OperationResult{T}"/> containing the updated transaction or an error type.</returns>
        Task<OperationResult<TransactionDto>> UpdateAsync(int id, UpdateTransactionDto model, int userId, bool isAdmin);

        /// <summary>
        /// Deletes the specified transaction from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the transaction to delete. Must be a positive integer.</param>
        /// <param name="userId">The ID of the user performing the deletion.</param>
        /// <param name="isAdmin">A flag indicating if the user is an administrator.</param>
        /// <returns>An <see cref="OperationResult{T}"/> indicating the outcome of the operation.</returns>
        Task<OperationResult<bool>> DeleteAsync(int id, int userId, bool isAdmin);
    }
}
