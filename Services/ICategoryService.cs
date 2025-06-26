using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ICategoryService
    {
        /// <summary>
        /// Adds a new category to the collection.
        /// </summary>
        /// <param name="model">The category to add. Cannot be null.</param>
        /// <returns><see langword="true"/> if the category was successfully added; otherwise, <see langword="false"/>.         </returns>
        bool Add(Category model);

        /// <summary>
        /// Retrieves a collection of categories associated with the specified user and globals.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose categories are to be retrieved. Must be a positive integer.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Category"/> objects representing the categories linked to the
        /// specified user. Returns only globals categories (empty Colletcion if theres no global categories) if the user has no associated categories.</returns>
        IEnumerable<Category> GetcategoryByUserId(int userId);

        /// <summary>
        /// Retrieves the category associated with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="Category"/> object corresponding to the specified identifier,  or <see langword="null"/> if
        /// no category with the given identifier exists.</returns>
        Category GetCategoryById(int id);

        /// <summary>
        /// Updates an existing category with the provided data.
        /// </summary>
        /// <param name="model">The category model containing updated information. Must not be null.</param>
        /// <returns>The updated category object.</returns>
        Category Update(Category model);

        /// <summary>
        /// Deletes the specified category from the system.
        /// </summary>
        /// <param name="model">The category to delete. Must not be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the category was successfully deleted; otherwise, <see langword="false"/>.</returns>
        bool Delete(Category model);
    }
}
