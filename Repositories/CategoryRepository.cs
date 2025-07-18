﻿﻿using Common;
using Data;
using Dtos.Category;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;

namespace Repositories
{
    public class CategoryRepository(ProjectDBContext context) : ICategoryService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));

        public async Task<CategoryDto> AddAsync(CreateCategoryDto model, int creatorId, bool isCreatorAdmin)
        {
            ArgumentNullException.ThrowIfNull(model);
            var category = new Category
            {
                Name = model.Name,
                Type = model.Type,
                // Si el creador es admin Y ha especificado un UserId, se la asigna a ese usuario.
                // Si es admin y no especifica UserId, es global (null).
                // Si no es admin, se la asigna a sí mismo.
                UserId = isCreatorAdmin ? model.UserId : creatorId
            };
            ValidateCategory(category);
            await _dbContext.Categories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
            return MapToDto(category);
        }

        public async Task<OperationResult<bool>> DeleteAsync(int id, int userId, bool isAdmin)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category is null)
                return OperationResult<bool>.Fail(Result.NotFound);

            if (!isAdmin && category.UserId is not null && category.UserId != userId)
                return OperationResult<bool>.Fail(Result.Forbidden);

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            return OperationResult<bool>.Success(true);
        }

        public async Task<IEnumerable<CategoryDto>> GetCategoriesByUserIdAsync(int userId, string? nameFilter = null, string? typeFilter = null)
        {
            // Consulta para obtener las categorías asociadas al usuario y las globales (UserId == null).
            var query = _dbContext.Categories.Where(c => c.UserId == userId || c.UserId == null);

            // Si se proporciona un filtro de nombre, lo aplicamos a la consulta ANTES de ejecutarla.
            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter.ToLower()));

            // Si se proporciona un filtro de tipo, lo aplicamos también.
            if (!string.IsNullOrWhiteSpace(typeFilter))
                query = query.Where(c => c.Type.ToLower().Equals(typeFilter.ToLower()));

            return await query.Select(c => MapToDto(c)).ToListAsync();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category is null)
                return null;

            return MapToDto(category);
        }


        public async Task<OperationResult<CategoryDto>> UpdateAsync(int id, UpdateCategoryDto model, int userId, bool isAdmin)
        {
            ArgumentNullException.ThrowIfNull(model);
            var categoryInDb = await _dbContext.Categories.FindAsync(id);
            if (categoryInDb is null)
                return OperationResult<CategoryDto>.Fail(Result.NotFound);

            if (!isAdmin && categoryInDb.UserId is not null && categoryInDb.UserId != userId)
                return OperationResult<CategoryDto>.Fail(Result.Forbidden);
            
            categoryInDb.Name = model.Name;
            categoryInDb.Type = model.Type;
            ValidateCategory(categoryInDb);
            await _dbContext.SaveChangesAsync();
            return OperationResult<CategoryDto>.Success(MapToDto(categoryInDb));
        }

        /// <summary>
        /// Validates the category type.
        /// </summary>
        /// <param name="model">The <see cref="Category"/> instance to validate.</param>
        /// <exception cref="ArgumentException">Thrown if the category type is not one of the valid types.</exception>
        private static void ValidateCategory(Category model)
        {
            model.Type = model.Type.ToUpper();
            if (!Category.ValidTypes.Contains(model.Type))
            {
                string validTypesString = string.Join("', '", Category.ValidTypes);
                throw new ArgumentException($"El tipo de categoría debe ser uno de los siguientes: '{validTypesString}'.", nameof(model));
            }
        }

        /// <summary>
        /// Maps a <see cref="Category"/> entity to a <see cref="CategoryDto"/>.
        /// </summary>
        /// <param name="category">The category entity to map.</param>
        /// <returns>A new <see cref="CategoryDto"/> instance.</returns>
        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                UserId = category.UserId
            };
        }
    }
}
