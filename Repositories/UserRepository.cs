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
    public class UserRepository(ProjectDBContext context) : IUserService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));

        public bool Add(User model)
        {
            ArgumentNullException.ThrowIfNull(model);
            _dbContext.Users.Add(model);
            _dbContext.SaveChanges();
            return true;
        }

        public bool Delete(int id)
        {
            var user = this.GetUser(u => u.Id == id);
            // Devolvemos 'false' para que el controlador pueda manejar cuando no se encontro un usuario (ej. con un 404 Not Found).
            if (user is null)
            {
                return false;
            }

            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();
            return true;
        }

        public IEnumerable<User> GetAllUsers() => _dbContext.Users.ToList();

        public User? GetUser(Expression<Func<User, bool>> filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            // Devuelve el usuario o null si no se encuentra.
            return _dbContext.Users.FirstOrDefault(filter);
        }

        public User? Update(User model)
        {
            ArgumentNullException.ThrowIfNull(model);
            var userInDb = _dbContext.Users.Find(model.Id);
            // Devolvemos 'null' para que el controlador pueda manejar cuando no se encontro un usuario (ej. con un 404 Not Found).
            if (userInDb is null)
            {
                return null;
            }
            userInDb.Name = model.Name;
            userInDb.Password = model.Password;
            _dbContext.SaveChanges();
            return userInDb;
        }
    }
}
