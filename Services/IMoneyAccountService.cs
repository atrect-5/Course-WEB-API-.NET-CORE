using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IMoneyAccountService
    {
        /// <summary>
        /// Adds a new money account to the system.
        /// </summary>
        /// <param name="model">The money account to add. Cannot be null.</param>
        /// <returns><see langword="true"/> if the money account was successfully created; otherwise, <see langword="false"/></returns>
        bool Add(MoneyAccount model);

        /// <summary>
        /// Retrieves a collection of all money accounts associated with the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose money accounts are to be retrieved. Must be a positive integer.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="MoneyAccount"/> objects representing the user's money accounts. Returns an empty collection if no accounts are found.</returns>
        IEnumerable<MoneyAccount> GetMoneyAccountsByUserId(int userId);

        /// <summary>
        /// Retrieves a money account by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the money account to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="MoneyAccount"/> object corresponding to the specified identifier, or <see langword="null"/> if no account with the given identifier exists.</returns>
        MoneyAccount GetMoneyAccountById(int id);

        /// <summary>
        /// Updates an existing money account with new information.
        /// </summary>
        /// <param name="model">The updated money account model. Must not be null and must include a valid identifier.</param>
        /// <returns>The updated <see cref="MoneyAccount"/> object reflecting the changes, or <see langword="null"/> if the update operation fails or the account does not exist.</returns>
        MoneyAccount Update(MoneyAccount model);

        /// <summary>
        /// Deletes the specified money account from the system.
        /// </summary>
        /// <param name="id">The unique identifier of the money account to delete. Must be a positive integer.</param>
        /// <returns><see langword="true"/> if the money account was successfully deleted; otherwise, <see langword="false"/>.</returns>
        bool Delete(int id);
    }
}
