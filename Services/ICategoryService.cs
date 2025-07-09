﻿using Dtos.Category;

namespace Services
{
    public interface ICategoryService
    {
        /// <summary>
        /// Adds a new category to the collection.
        /// </summary>
        /// <param name="model">The category to add. Cannot be null.</param>
        /// <returns>A <see cref="CategoryDto"/> representing the newly created category.</returns>
        Task<CategoryDto> AddAsync(CreateCategoryDto model);

        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="nameFilter">Optional. A name to filter the categories by.</param>
        /// <param name="typeFilter">Optional. The type to filter categories by (e.g., "INCOME" or "EXPENDITURE").</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="CategoryDto"/> objects linked to the user and any global 
        /// categories, optionally filtered by name and/or type.</returns>
        Task<IEnumerable<CategoryDto>> GetCategoriesByUserIdAsync(int userId, string? nameFilter = null, string? typeFilter = null);

        /// <summary>
        /// Retrieves the category associated with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the category to retrieve. Must be a positive integer.</param>
        /// <returns>The <see cref="CategoryDto"/> object corresponding to the specified <paramref name="id"/>,  or <see
        /// langword="null"/> if no category with the given identifier exists.</returns>
        Task<CategoryDto?> GetCategoryByIdAsync(int id);

        /// <summary>
        /// Updates an existing category with the provided data.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="model">The DTO containing the updated information.</param>
        /// <returns>The updated category DTO, or <see langword="null"/> if the category to update is not found.</returns>
        Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto model);

        /// <summary>
        /// Deletes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete. Must be a positive integer.</param>
        /// <returns><see langword="true"/> if the entity was successfully deleted; otherwise, <see langword="false"/>.</returns>
        Task<bool> DeleteAsync(int id);
    }
}
