using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ITransferService
    {
        /// <summary>
        /// Adds a new transfer to the system.
        /// </summary>
        /// <param name="model">The transfer model to be added. Must not be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the transfer was successfully added; otherwise, <see langword="false"/>.</returns>
        bool Add(Transfer model);

        /// <summary>
        /// Retrieves a collection of transfers associated with the specified user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose transfers are to be retrieved.</param>
        /// <returns>An <see cref="IEnumerable{Transfer}"/> containing the transfers for the specified user. If the user has no
        /// transfers, the collection will be empty.</returns>
        IEnumerable<Transfer> GetTransfersByUserId(int userId);

        /// <summary>
        /// Retrieves a collection of transfers sent from the specified money account.
        /// </summary>
        /// <param name="moneyAccountId">The unique identifier of the money account whose sent transfers are to be retrieved. Must be a positive
        /// integer.</param>
        /// <returns>An <see cref="IEnumerable{Transfer}"/> containing the transfers sent from the specified money account.
        /// Returns an empty collection if no transfers are found.</returns>
        IEnumerable<Transfer> GetTransfersSentByIdAccount(int moneyAccountId);

        /// <summary>
        /// Retrieves a collection of transfers received by the specified money account.
        /// </summary>
        /// <param name="moneyAccountId">The unique identifier of the money account for which to retrieve received transfers. Must be a positive
        /// integer.</param>
        /// <returns>An <see cref="IEnumerable{Transfer}"/> containing the transfers received by the specified money account.
        /// Returns an empty collection if no transfers are found.</returns>
        IEnumerable<Transfer> GetTransfersReceivedByIdAccount(int moneyAccountId);

        /// <summary>
        /// Retrieves a Transfer by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the Transfer to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="Transfer"/> object corresponding to the specified identifier, or <see langword="null"/> if no Transfer with the given identifier exists.</returns>
        Transfer GetTransferById(int id);

        /// <summary>
        /// Retrieves a collection of Transfer for a specified user within a given date range.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose Transfers are to be retrieved. Must be a positive integer.</param>
        /// <param name="startDate">The start date of the date range. Transfers occurring on or after this date will be included.</param>
        /// <param name="endDate">The end date of the date range. Transfers occurring on or before this date will be included.</param>
        /// <returns>An enumerable collection of <see cref="Transfer"/> objects representing the user's Transfers within
        /// the specified date range. Returns an empty collection if no Transfers are found.</returns>
        IEnumerable<Transfer> GetTransferByDateRange(int userId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Updates an existing Transfer with new information.
        /// </summary>
        /// <param name="model">The updated Transfer model. Must not be null and must include a valid identifier.</param>
        /// <returns>The updated <see cref="Transfer"/> object reflecting the changes, or <see langword="null"/> if the update operation fails or the Transfer does not exist.</returns>
        Transfer Update(Transfer model);

        /// <summary>
        /// Deletes the specified Transfer from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the Transfer to delete. Must be a positive integer.</param>
        /// <returns><see langword="true"/> if the Transfer was successfully deleted; otherwise, <see langword="false"/>.</returns>
        bool Delete(int id);
    }
}
