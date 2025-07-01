using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// <returns><see langword="true"/> if the category was successfully added; otherwise, <see langword="false"/>.</returns>
        bool Add(Category model);

        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="nameFilter">Optional. A name to filter the categories by.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Category"/> objects linked to the user and any global 
        /// categories, optionally filtered by name.</returns>
        IEnumerable<Category> GetCategoriesByUserId(int userId, string? nameFilter = null);

        /// <summary>
        /// Retrieves the category associated with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="Category"/> object corresponding to the specified <paramref name="id"/>,  or <see
        /// langword="null"/> if no category with the given identifier exists.</returns>
        Category? GetCategoryById(int id);

        /// <summary>
        /// Updates an existing category with the provided data.
        /// </summary>
        /// <param name="model">The category model containing updated information. Must not be null.</param>
        /// /// <returns>The updated category object, or <see langword="null"/> if the category to update is not found.</returns>
        Category? Update(Category model);

        /// <summary>
        /// Deletes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete. Must be a positive integer.</param>
        /// <returns><see langword="true"/> if the entity was successfully deleted; otherwise, <see langword="false"/>.</returns>
        bool Delete(int id);
    }
}
