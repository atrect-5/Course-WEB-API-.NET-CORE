using Common;
using Dtos.MoneyAccount;

namespace Services
{
    public interface IMoneyAccountService
    {
        /// <summary>
        /// Adds a new money account to the system.
        /// </summary>
        /// <param name="model">The money account to add. Cannot be null.</param>
        /// <param name="creatorId">The ID of the user creating the account.</param>
        /// <param name="isCreatorAdmin">A flag indicating if the user is an administrator.</param>
        /// <returns>A <see cref="MoneyAccountDto"/> representing the newly created MoneyAccount.</returns>
        Task<MoneyAccountDto> AddAsync(CreateMoneyAccountDto model,  int creatorId, bool isCreatorAdmin);

        /// <summary>
        /// Retrieves a collection of money accounts associated with the specified user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose money accounts are to be retrieved. Must be a positive integer.</param>
        /// <param name="nameFilter">An optional filter to retrieve accounts whose names contain the specified substring. If null, no name-based filtering is applied.</param>
        /// <param name="typeFilter">An optional filter to retrieve accounts of a specific type. If null, no type-based filtering is applied.</param>
        /// <returns>A collection of <see cref="MoneyAccountDto"/> objects associated with the specified user ID. Returns an empty
        /// collection if no accounts match the criteria.</returns>
        Task<IEnumerable<MoneyAccountDto>> GetMoneyAccountsByUserIdAsync(int userId, string? nameFilter = null, string? typeFilter = null);

        /// <summary>
        /// Retrieves a money account by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the money account to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="MoneyAccountDto"/> object corresponding to the specified identifier, or null if no account with the given identifier exists.</returns>
        Task<MoneyAccountDto?> GetMoneyAccountByIdAsync(int id);

        /// <summary>
        /// Updates an existing money account with new information.
        /// </summary>
        /// <param name="id">The ID of the account to update.</param>
        /// <param name="model">The DTO with the updated account information.</param>
        /// <param name="userId">The ID of the user performing the update.</param> 
        /// <param name="isAdmin">A flag indicating if the user is an administrator.</param> 
        /// <returns>An <see cref="OperationResult{T}"/> containing the updated account or an error type.</returns>
        Task<OperationResult<MoneyAccountDto>> UpdateAsync(int id, UpdateMoneyAccountDto model, int userId, bool isAdmin);

        /// <summary>
        /// Deletes the specified money account from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the money account to delete. Must be a positive integer.</param>
        /// <param name="userId">The ID of the user performing the deletion.</param> 
        /// <param name="isAdmin">A flag indicating if the user is an administrator.</param> 
        /// <returns>An <see cref="OperationResult{T}"/> indicating the outcome of the operation.</returns>
        Task<OperationResult<bool>> DeleteAsync(int id, int userId, bool isAdmin);
    }
}
