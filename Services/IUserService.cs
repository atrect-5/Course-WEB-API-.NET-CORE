using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IUserService
    {
        /// <summary>
        /// Adds a new user to the system.
        /// </summary>
        /// <remarks>
        /// This method adds the specified user to the system. Ensure that the user object
        /// contains valid data  before calling this method.
        /// </remarks>
        /// <param name="model">The user to add. Cannot be null.</param>
        /// <returns><see langword="true"/> if the entity was successfully created; otherwise, <see langword="false"/></returns>
        bool Add(User model);

        /// <summary>
        /// Retrieves a Collection of all users.
        /// </summary>
        /// <remarks>This method does not filter or paginate the results. It retrieves all users in a
        /// single operation.</remarks>
        /// <returns>A Collection of <see cref="User"/> objects representing all users.  Returns an empty Collection if no users are
        /// available.</returns>
        IEnumerable<User> GetAllUsers();

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="User"/> object corresponding to the specified <paramref name="id"/>,  or <see
        /// langword="null"/> if no user with the given identifier exists.</returns>
        User GetUserById(int id);
        
        /// <summary>
        /// Updates the specified user with new information.
        /// </summary>
        /// <param name="model">The <see cref="User"/> object containing the updated user information.  The <paramref name="model"/> must
        /// not be null and must include a valid user identifier.</param>
        /// <returns>The updated <see cref="User"/> object reflecting the changes.  Returns <see langword="null"/> if the update
        /// operation fails or the user does not exist.</returns>
        User Update(User model);

        /// <summary>
        /// Deletes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete. Must be a positive integer.</param>
        /// <returns><see langword="true"/> if the entity was successfully deleted; otherwise, <see langword="false"/>.</returns>
        bool Delete(int id);
    }
}
