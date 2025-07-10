using Data;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Repositories
{
    public class AuthenticationRepository(ProjectDBContext context) : IAuthenticationService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        public async Task<string?> AuthenticateAsync(string email, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null; // Autenticación fallida.

            //return new UserDto { Id = user.Id, Name = user.Name, Email = user.Email };
            return "JWT Token";
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
