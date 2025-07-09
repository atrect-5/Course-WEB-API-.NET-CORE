using Dtos.Transfer;

namespace Services
{
    public interface ITransferService
    {
        /// <summary>
        /// Adds a new transfer to the system.
        /// </summary>
        /// <param name="model">The DTO for creating the transfer. Must not be <see langword="null"/>.</param>
        /// <returns>A <see cref="TransferDto"/> representing the newly created transfer.</returns>
        Task<TransferDto> AddAsync(CreateTransferDto model);

        /// <summary>
        /// Retrieves a Transfer by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the Transfer to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="TransferDto"/> object corresponding to the specified identifier, or <see langword="null"/> if no Transfer with the given identifier exists.</returns>
        Task<TransferDto?> GetTransferByIdAsync(int id);

        /// <summary>
        /// Retrieves a collection of transfers associated with a specific user.
        /// </summary>
        /// <remarks>If no optional parameters are provided, all transfers associated with the user will
        /// be retrieved. Use the optional parameters to narrow the results based on account, date range, or
        /// both.</remarks>
        /// <param name="userId">The unique identifier of the user whose transfers are to be retrieved.</param>
        /// <param name="moneyAccountId">An optional parameter specifying the unique identifier of a money account.  If provided, only transfers
        /// related to this account will be included.</param>
        /// <param name="startDate">An optional parameter specifying the start date for filtering transfers.  Transfers occurring on or after
        /// this date will be included.</param>
        /// <param name="endDate">An optional parameter specifying the end date for filtering transfers.  Transfers occurring on or before
        /// this date will be included.</param>
        /// <returns>A collection of <see cref="TransferDto"/> objects representing the user's transfers.  The collection will be
        /// empty if no transfers match the specified criteria.</returns>
        Task<IEnumerable<TransferDto>> GetTransfersByUserIdAsync(int userId, int? moneyAccountId = null, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Deletes the specified Transfer from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the Transfer to delete. Must be a positive integer.</param>
        /// <returns><see langword="true"/> if the Transfer was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeleteAsync(int id);
    }
}
