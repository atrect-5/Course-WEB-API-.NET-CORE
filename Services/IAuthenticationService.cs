using System;
using System.Threading.Tasks;

namespace Services
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Authenticates a user and also creates a refresh token.
        /// Returns null on failure; otherwise returns JWT, raw refresh token, refresh expiry and user id.
        /// The caller should pass the desired refresh token lifespan in days.
        /// </summary>
        Task<(string Jwt, string RefreshToken, DateTime RefreshExpiresAt, int UserId)?> AuthenticateAsync(string email, string password, string? createdByIp = null, string? userAgent = null, int refreshValidDays = 45);

        /// <summary>
        /// Logout: revoke the provided raw refresh token (if any).
        /// </summary>
        Task LogoutAsync(string? rawRefreshToken);
    }
}
