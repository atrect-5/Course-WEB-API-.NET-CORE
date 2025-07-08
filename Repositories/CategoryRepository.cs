﻿using Data;
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

        public bool Add(Category model)
        {
            ArgumentNullException.ThrowIfNull(model);
            ValidateCategory(model);
            _dbContext.Categories.Add(model);
            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var category = _dbContext.Categories.Find(id);
            if (category is null)
            {
                return false;
            }

            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();
            return true;
        }

        public IEnumerable<Category> GetCategoriesByUserId(int userId, string? nameFilter = null, string? typeFilter = null)
        {
            // Consulta para obtener las categorías asociadas al usuario y las globales (UserId == null).
            var query = _dbContext.Categories.Where(c => c.UserId == userId || c.UserId == null);

            // Si se proporciona un filtro de nombre, lo aplicamos a la consulta ANTES de ejecutarla.
            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                // Usamos la sobrecarga con StringComparison para una búsqueda eficiente y no sensible a mayúsculas/minúsculas.
                query = query.Where(c => c.Name.ToLower().Contains(nameFilter.ToLower()));
            }

            // Si se proporciona un filtro de tipo, lo aplicamos también.
            if (!string.IsNullOrWhiteSpace(typeFilter))
            {
                query = query.Where(c => c.Type.ToLower().Equals(typeFilter.ToLower()));
            }

            return query.ToList();
        }

        public Category? GetCategoryById(int id) => 
            _dbContext.Categories.Find(id);


        public Category? Update(Category model)
        {
            ArgumentNullException.ThrowIfNull(model);
            ValidateCategory(model);
            var categoryInDb = _dbContext.Categories.Find(model.Id);
            if (categoryInDb is null)
            {
                return null;
            }
            categoryInDb.Name = model.Name;
            categoryInDb.Type = model.Type;
            _dbContext.SaveChanges();
            return categoryInDb;
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
