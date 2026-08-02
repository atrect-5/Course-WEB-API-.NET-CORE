using Data;
using Dtos;
using Microsoft.EntityFrameworkCore;
using Models;
using Services;
using System;
using System.Threading.Tasks;

namespace Repositories
{
    public class AuthenticationRepository(ProjectDBContext context, ITokenService tokenService, IRefreshTokenService refreshTokenService) : IAuthenticationService
    {
        private readonly ProjectDBContext _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService ?? throw new ArgumentNullException(nameof(refreshTokenService));

        public async Task<(string Jwt, string RefreshToken, DateTime RefreshExpiresAt, int UserId)?> AuthenticateAsync(string email, string password, string? createdByIp = null, string? userAgent = null, int refreshValidDays = 45)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                return null; // Autenticación fallida.

            // Generate JWT
            var jwt = _tokenService.GenerateJWToken(user);

            // Create refresh token record (returns raw token and expiry)
            var (refreshToken, refreshExpires) = await _refreshTokenService.CreateRefreshTokenAsync(user.Id, createdByIp ?? string.Empty, userAgent, validDays: refreshValidDays);

            return (Jwt: jwt, RefreshToken: refreshToken, RefreshExpiresAt: refreshExpires, UserId: user.Id);
        }

        public async Task LogoutAsync(string? rawRefreshToken)
        {
            if (string.IsNullOrEmpty(rawRefreshToken))
            {
                // Nothing to revoke server-side
                return;
            }

            var tokenRecord = await _refreshTokenService.GetByRawTokenAsync(rawRefreshToken);
            if (tokenRecord is null) return;

            await _refreshTokenService.RevokeAsync(tokenRecord);
        }
    }
}
