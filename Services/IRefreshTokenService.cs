using Models;
using System;
using System.Threading.Tasks;

namespace Services
{
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Creates and persists a new refresh token for the given user.
        /// Returns the raw token (to be set in an HttpOnly cookie) and its expiry.
        /// The caller MUST provide the number of valid days.
        /// </summary>
        Task<(string Token, DateTime ExpiresAt)> CreateRefreshTokenAsync(int userId, string createdByIp, string? userAgent, int validDays);

        /// <summary>
        /// Finds a refresh token record by raw token value (computes hash internally).
        /// Includes navigation to User.
        /// </summary>
        Task<RefreshToken?> GetByRawTokenAsync(string token);

        /// <summary>
        /// Marks the provided token record as revoked. Optionally set the hash of the token that replaced it.
        /// </summary>
        Task RevokeAsync(RefreshToken tokenRecord, string? replacedByTokenHash = null);

        /// <summary>
        /// Rotates an existing refresh token: revokes the old one and creates a new persisted one.
        /// Returns the raw new token and its expiry.
        /// The caller MUST provide the number of valid days for the new token.
        /// </summary>
        Task<(string NewToken, DateTime ExpiresAt)> RotateAsync(RefreshToken existingToken, string createdByIp, string? userAgent, int validDays);

        /// <summary>
        /// Revoke all active refresh tokens for a user (used on suspicious activity / logout-all).
        /// </summary>
        Task RevokeAllForUserAsync(int userId);
    }
}
