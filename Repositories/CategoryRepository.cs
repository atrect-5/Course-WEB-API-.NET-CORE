﻿using Data;
using Dtos.Category;
using Models;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class CategoryRepository(ProjectDBContext context) : ICategoryService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));

        public CategoryDto Add(CreateCategoryDto model)
        {
            ArgumentNullException.ThrowIfNull(model);
            var category = new Category
            {
                Name = model.Name,
                Type = model.Type,
                UserId = model.UserId
            };
            ValidateCategory(category);
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                UserId = category.UserId
            };
        }

        public bool Delete(int id)
        {
            var category = _dbContext.Categories.Find(id);
            if (category is null)
                return false;

            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();
            return true;
        }

        public IEnumerable<CategoryDto> GetCategoriesByUserId(int userId, string? nameFilter = null, string? typeFilter = null)
        {
            // Consulta para obtener las categorías asociadas al usuario y las globales (UserId == null).
            var query = _dbContext.Categories.Where(c => c.UserId == userId || c.UserId == null);

            // Si se proporciona un filtro de nombre, lo aplicamos a la consulta ANTES de ejecutarla.
            if (!string.IsNullOrWhiteSpace(nameFilter))
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter.ToLower()));

            // Si se proporciona un filtro de tipo, lo aplicamos también.
            if (!string.IsNullOrWhiteSpace(typeFilter))
                query = query.Where(c => c.Type.ToLower().Equals(typeFilter.ToLower()));

            return query.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type,
                UserId = c.UserId
            }).ToList();
        }

        public CategoryDto? GetCategoryById(int id)
        {
            var category = _dbContext.Categories.Find(id);
            if (category is null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                UserId = category.UserId
            };
        }


        public CategoryDto? Update(int id, UpdateCategoryDto model)
        {
            ArgumentNullException.ThrowIfNull(model);
            var categoryInDb = _dbContext.Categories.Find(id);
            if (categoryInDb is null)
                return null;
            
            categoryInDb.Name = model.Name;
            categoryInDb.Type = model.Type;
            ValidateCategory(categoryInDb);
            _dbContext.SaveChanges();
            return new CategoryDto
            {
                Id = categoryInDb.Id,
                Name = categoryInDb.Name,
                Type = categoryInDb.Type,
                UserId = categoryInDb.UserId
            };
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
    }
}
