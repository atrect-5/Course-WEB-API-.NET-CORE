using Data;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Services;

namespace Repositories
{
    public class AuthenticationRepository(ProjectDBContext context, ITokenService tokenService) : IAuthenticationService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException( nameof(tokenService));
        public async Task<string?> AuthenticateAsync(string email, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null; // Autenticación fallida.

            return _tokenService.GenerateToken(user);
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
