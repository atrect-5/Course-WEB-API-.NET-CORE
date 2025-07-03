using Data;
using Dtos;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class AuthenticationRepository(ProjectDBContext context) : IAuthenticationService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        public UserDto? Authenticate(string email, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null; // Autenticación fallida.
            }

            return new UserDto { Id = user.Id, Name = user.Name, Email = user.Email };
        }

        public void Logout()
        {
            throw new NotImplementedException();
        }
    }
}
