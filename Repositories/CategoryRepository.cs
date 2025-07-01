using Data;
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

        public IEnumerable<Category> GetCategoriesByUserId(int userId, string? nameFilter = null)
        {
            // Consulta para obtener las categorías asociadas al usuario y las globales (UserId == null).
            var query = _dbContext.Categories.Where(c => c.UserId == userId || c.UserId == null);

            // Si se proporciona un filtro de nombre, lo aplicamos a la consulta ANTES de ejecutarla.
            if (!string.IsNullOrWhiteSpace(nameFilter))
            {
                query = query.Where(c => c.Name.Contains(nameFilter));
            }

            return query.ToList();
        }

        public Category? GetCategoryById(int id)
        {
            return _dbContext.Categories.Find(id);
        }

        public Category? Update(Category model)
        {
            ArgumentNullException.ThrowIfNull(model);
            var categoryInDb = _dbContext.Categories.Find(model.Id);
            if (categoryInDb is null)
            {
                return null;
            }
            categoryInDb.Name = model.Name;
            _dbContext.SaveChanges();
            return categoryInDb;
        }
    }
}
